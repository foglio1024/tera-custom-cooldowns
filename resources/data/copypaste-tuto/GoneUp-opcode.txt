Guide to the Opcode DLL:

Code from tagyourit50, released at ragezone: http://forum.ragezone.com/f797/release-tera-live-packet-sniffer-1052922-post8369480/#post8369480


#########
Post Copy:
For those wondering how to get the opcode names here's a little guide.

1. In cheat engine, find the function that references the string "I_TELEPORT". Look just above that and find the start of the function. (In this case 0x0191D0A0)
--Use the view -> referenced strings function, search for I_TELEPORT there.


2. Make a c++ dll that looks like http://pastebin.com/qTGzrW8w with the address that you got in the previous step. 

3. Now inject it into the Tera process, after which you should see some message boxes.
Click OK on those, then in your TERA\Client\Binaries folder you should see a file called opcodes.txt full of opcodes. 
(You can just change the output directory if you want)

#######


If you want to use the file with the packet editor, use the decimal output file, copy it to the packetviewer directory and name it "opcodefile.txt"

######
GoneUp
