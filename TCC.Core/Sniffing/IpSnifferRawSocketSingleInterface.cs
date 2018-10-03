// Copyright (c) CodesInChaos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using PacketDotNet;
using PacketDotNet.Utils;

namespace TCC.Sniffing
{
    public class IpSnifferRawSocketSingleInterface : IpSniffer
    {
        private readonly IPAddress _localIp;

        private bool _isInit;
        private Socket _socket;

        public IpSnifferRawSocketSingleInterface(IPAddress localIp)
        {
            _localIp = localIp;
        }

        private void Init()
        {
            Debug.Assert(_socket == null);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);

            if (_localIp != null)
            {
                _socket.Bind(new IPEndPoint(_localIp, 0));
            }
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
            var receiveAllIp = BitConverter.GetBytes(3);
            _socket.IOControl(IOControlCode.ReceiveAll, receiveAllIp, null);
            _socket.ReceiveBufferSize = 1 << 24;
            Task.Run(()=>ReadAsync(_socket));
        }

        private async Task ReadAsync(Socket s)
        {
            // Reusable SocketAsyncEventArgs and awaitable wrapper 
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(new byte[0x100000], 0, 0x100000);
            var awaitable = new SocketAwaitable(args);
            while (true)
            {
                await s.ReceiveAsync(awaitable);
                var bytesRead = args.BytesTransferred;
                if (bytesRead <= 0) throw new Exception("Raw socket is disconnected");
                var ipPacket = new IPv4Packet(new ByteArraySegment(args.Buffer, 0, bytesRead));
                if (ipPacket.Version != IpVersion.IPv4 || ipPacket.Protocol!=IPProtocolType.TCP)
                    continue;
                OnPacketReceived(ipPacket);
            }
        }

        private void Finish()
        {
            if (!_isInit)
            {
                return;
            }
            Debug.Assert(_socket != null);
            _socket.Close();
            _socket = null;
        }
        
        protected override void SetEnabled(bool value)
        {
            if (value)
            {
                try
                {
                    Init();
                    _isInit = true;
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                Finish();
            }
        }


        public override string ToString()
        {
            return $"{base.ToString()} {_localIp}";
        }
    }

    public sealed class SocketAwaitable : INotifyCompletion
    {
        private static readonly Action Sentinel = () => { };

        internal bool MWasCompleted;
        private Action _mContinuation;
        internal readonly SocketAsyncEventArgs MEventArgs;

        public SocketAwaitable(SocketAsyncEventArgs eventArgs)
        {
            MEventArgs = eventArgs ?? throw new ArgumentNullException("eventArgs");
            eventArgs.Completed += delegate
            {
                (_mContinuation ?? Interlocked.CompareExchange(
                    ref _mContinuation, Sentinel, null))?.Invoke();
            };
        }

        internal void Reset()
        {
            MWasCompleted = false;
            _mContinuation = null;
        }

        public SocketAwaitable GetAwaiter() { return this; }

        public bool IsCompleted => MWasCompleted;

        public void OnCompleted(Action continuation)
        {
            if (_mContinuation == Sentinel ||
                Interlocked.CompareExchange(
                    ref _mContinuation, continuation, null) == Sentinel)
            {
                Task.Run(continuation);
            }
        }

        public void GetResult()
        {
            if (MEventArgs.SocketError != SocketError.Success)
                throw new SocketException((int)MEventArgs.SocketError);
        }
    }
    public static class SocketExtensions
    {
        public static SocketAwaitable ReceiveAsync(this Socket socket,
            SocketAwaitable awaitable)
        {
            awaitable.Reset();
            if (!socket.ReceiveAsync(awaitable.MEventArgs))
                awaitable.MWasCompleted = true;
            return awaitable;
        }
    }
}