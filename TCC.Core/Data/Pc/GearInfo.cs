using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Newtonsoft.Json;
using TeraDataLite;

namespace TCC.Data.Pc
{
    public class GearInfo
    {
        [JsonIgnore] public GearItem Weapon => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Weapon) ?? new GearItem(0, GearTier.Low, GearPiece.Weapon, 0, 0);
        [JsonIgnore] public GearItem Chest => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Armor) ?? new GearItem(0, GearTier.Low, GearPiece.Armor, 0, 0);
        [JsonIgnore] public GearItem Hands => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Hands) ?? new GearItem(0, GearTier.Low, GearPiece.Hands, 0, 0);
        [JsonIgnore] public GearItem Feet => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Feet) ?? new GearItem(0, GearTier.Low, GearPiece.Feet, 0, 0);
        [JsonIgnore] public GearItem Belt => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Belt) ?? new GearItem(0, GearTier.Low, GearPiece.Belt, 0, 0);
        [JsonIgnore] public GearItem Circlet => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Circlet) ?? new GearItem(0, GearTier.Low, GearPiece.Circlet, 0, 0);
        [JsonIgnore] public ICollectionView Jewels { get; set; }
        public SynchronizedObservableCollection<GearItem> Gear { get; set; }

        public GearInfo()
        {
            Gear = new SynchronizedObservableCollection<GearItem>();
            Jewels = new CollectionViewSource() { Source = Gear }.View;
            Jewels.Filter = g => ((GearItem)g).IsJewel && ((GearItem)g).Piece < GearPiece.Circlet;
            Jewels.SortDescriptions.Add(new SortDescription("Piece", ListSortDirection.Ascending));
        }

    }
}