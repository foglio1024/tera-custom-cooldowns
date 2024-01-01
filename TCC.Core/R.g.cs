///////////////////////////////////////////////////////////
//// File automatically generated from TCC.Core.csproj ////
///////////////////////////////////////////////////////////

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Dragablz.Converters;
using Nostrum.WPF.Converters;
using TCC.UI.Converters;
using TCC.UI.TemplateSelectors;

namespace TCC.R;

// ResourceDictionaries/Brushes.xaml
public class Brushes : RH
{
    public static LinearGradientBrush RevampBackgroundGradientBrush => Get<LinearGradientBrush>("RevampBackgroundGradientBrush");
    public static LinearGradientBrush TccGreenGradientBrush => Get<LinearGradientBrush>("TccGreenGradientBrush");
    public static LinearGradientBrush TccNormalGradientBrush => Get<LinearGradientBrush>("TccNormalGradientBrush");
    public static LinearGradientBrush TccRedGradientBrush => Get<LinearGradientBrush>("TccRedGradientBrush");
    public static LinearGradientBrush TccWhiteGradientBrush => Get<LinearGradientBrush>("TccWhiteGradientBrush");
    public static LinearGradientBrush TccYellowGradientBrush => Get<LinearGradientBrush>("TccYellowGradientBrush");
    public static SolidColorBrush AquadraxBrush => Get<SolidColorBrush>("AquadraxBrush");
    public static SolidColorBrush ArcaneBrush => Get<SolidColorBrush>("ArcaneBrush");
    public static SolidColorBrush ArcaneBrushLight => Get<SolidColorBrush>("ArcaneBrushLight");
    public static SolidColorBrush AssaultStanceBrush => Get<SolidColorBrush>("AssaultStanceBrush");
    public static SolidColorBrush AssaultStanceBrushDark => Get<SolidColorBrush>("AssaultStanceBrushDark");
    public static SolidColorBrush AssaultStanceBrushLight => Get<SolidColorBrush>("AssaultStanceBrushLight");
    public static SolidColorBrush BackgroundDarkBrush => Get<SolidColorBrush>("BackgroundDarkBrush");
    public static SolidColorBrush CardDarkBrush => Get<SolidColorBrush>("CardDarkBrush");
    public static SolidColorBrush ChatAreaBrush => Get<SolidColorBrush>("ChatAreaBrush");
    public static SolidColorBrush ChatEmoteBrush => Get<SolidColorBrush>("ChatEmoteBrush");
    public static SolidColorBrush ChatGlobalBrush => Get<SolidColorBrush>("ChatGlobalBrush");
    public static SolidColorBrush ChatGreetBrush => Get<SolidColorBrush>("ChatGreetBrush");
    public static SolidColorBrush ChatGuildAdBrush => Get<SolidColorBrush>("ChatGuildAdBrush");
    public static SolidColorBrush ChatGuildBrush => Get<SolidColorBrush>("ChatGuildBrush");
    public static SolidColorBrush ChatMegaphoneBrush => Get<SolidColorBrush>("ChatMegaphoneBrush");
    public static SolidColorBrush ChatMegaphoneBrushDark => Get<SolidColorBrush>("ChatMegaphoneBrushDark");
    public static SolidColorBrush ChatPartyBrush => Get<SolidColorBrush>("ChatPartyBrush");
    public static SolidColorBrush ChatPartyNoticeBrush => Get<SolidColorBrush>("ChatPartyNoticeBrush");
    public static SolidColorBrush ChatPrivateBrush => Get<SolidColorBrush>("ChatPrivateBrush");
    public static SolidColorBrush ChatProxyBrush => Get<SolidColorBrush>("ChatProxyBrush");
    public static SolidColorBrush ChatRaidBrush => Get<SolidColorBrush>("ChatRaidBrush");
    public static SolidColorBrush ChatRaidNoticeBrush => Get<SolidColorBrush>("ChatRaidNoticeBrush");
    public static SolidColorBrush ChatSayBrush => Get<SolidColorBrush>("ChatSayBrush");
    public static SolidColorBrush ChatSystemBargainBrush => Get<SolidColorBrush>("ChatSystemBargainBrush");
    public static SolidColorBrush ChatSystemContractAlertBrush => Get<SolidColorBrush>("ChatSystemContractAlertBrush");
    public static SolidColorBrush ChatSystemDeathmatchBrush => Get<SolidColorBrush>("ChatSystemDeathmatchBrush");
    public static SolidColorBrush ChatSystemErrorBrush => Get<SolidColorBrush>("ChatSystemErrorBrush");
    public static SolidColorBrush ChatSystemEventBrush => Get<SolidColorBrush>("ChatSystemEventBrush");
    public static SolidColorBrush ChatSystemExpBrush => Get<SolidColorBrush>("ChatSystemExpBrush");
    public static SolidColorBrush ChatSystemFriendBrush => Get<SolidColorBrush>("ChatSystemFriendBrush");
    public static SolidColorBrush ChatSystemGenericBrush => Get<SolidColorBrush>("ChatSystemGenericBrush");
    public static SolidColorBrush ChatSystemGroupAlertBrush => Get<SolidColorBrush>("ChatSystemGroupAlertBrush");
    public static SolidColorBrush ChatSystemGroupBrush => Get<SolidColorBrush>("ChatSystemGroupBrush");
    public static SolidColorBrush ChatSystemLootBrush => Get<SolidColorBrush>("ChatSystemLootBrush");
    public static SolidColorBrush ChatSystemMoneyBrush => Get<SolidColorBrush>("ChatSystemMoneyBrush");
    public static SolidColorBrush ChatSystemNotifyBrush => Get<SolidColorBrush>("ChatSystemNotifyBrush");
    public static SolidColorBrush ChatSystemQuestBrush => Get<SolidColorBrush>("ChatSystemQuestBrush");
    public static SolidColorBrush ChatSystemWorldBossBrush => Get<SolidColorBrush>("ChatSystemWorldBossBrush");
    public static SolidColorBrush ChatTradeBrush => Get<SolidColorBrush>("ChatTradeBrush");
    public static SolidColorBrush ChatWhisperBrush => Get<SolidColorBrush>("ChatWhisperBrush");
    public static SolidColorBrush CooldownArcBrush => Get<SolidColorBrush>("CooldownArcBrush");
    public static SolidColorBrush CopperBrush => Get<SolidColorBrush>("CopperBrush");
    public static SolidColorBrush DailyBrush => Get<SolidColorBrush>("DailyBrush");
    public static SolidColorBrush DarkGreenBrush => Get<SolidColorBrush>("DarkGreenBrush");
    public static SolidColorBrush DefaultBackgroundBrush => Get<SolidColorBrush>("DefaultBackgroundBrush");
    public static SolidColorBrush DefensiveStanceBrush => Get<SolidColorBrush>("DefensiveStanceBrush");
    public static SolidColorBrush DefensiveStanceBrushDark => Get<SolidColorBrush>("DefensiveStanceBrushDark");
    public static SolidColorBrush DefensiveStanceBrushLight => Get<SolidColorBrush>("DefensiveStanceBrushLight");
    public static SolidColorBrush DpsRoleBrush => Get<SolidColorBrush>("DpsRoleBrush");
    public static SolidColorBrush EnchantHighBrush => Get<SolidColorBrush>("EnchantHighBrush");
    public static SolidColorBrush EnchantLowBrush => Get<SolidColorBrush>("EnchantLowBrush");
    public static SolidColorBrush FireBrush => Get<SolidColorBrush>("FireBrush");
    public static SolidColorBrush FireBrushLight => Get<SolidColorBrush>("FireBrushLight");
    public static SolidColorBrush GoldBrush => Get<SolidColorBrush>("GoldBrush");
    public static SolidColorBrush GreenBrush => Get<SolidColorBrush>("GreenBrush");
    public static SolidColorBrush GuardianBrush => Get<SolidColorBrush>("GuardianBrush");
    public static SolidColorBrush HealerRoleBrush => Get<SolidColorBrush>("HealerRoleBrush");
    public static SolidColorBrush HpBrush => Get<SolidColorBrush>("HpBrush");
    public static SolidColorBrush HpBrushLight => Get<SolidColorBrush>("HpBrushLight");
    public static SolidColorBrush HpDebuffBrush => Get<SolidColorBrush>("HpDebuffBrush");
    public static SolidColorBrush HpDebuffBrushLight => Get<SolidColorBrush>("HpDebuffBrushLight");
    public static SolidColorBrush IceBrush => Get<SolidColorBrush>("IceBrush");
    public static SolidColorBrush IceBrushLight => Get<SolidColorBrush>("IceBrushLight");
    public static SolidColorBrush IgnidraxBrush => Get<SolidColorBrush>("IgnidraxBrush");
    public static SolidColorBrush ItemCommonBrush => Get<SolidColorBrush>("ItemCommonBrush");
    public static SolidColorBrush ItemHeroicBrush => Get<SolidColorBrush>("ItemHeroicBrush");
    public static SolidColorBrush ItemRareBrush => Get<SolidColorBrush>("ItemRareBrush");
    public static SolidColorBrush ItemSuperiorBrush => Get<SolidColorBrush>("ItemSuperiorBrush");
    public static SolidColorBrush ItemUncommonBrush => Get<SolidColorBrush>("ItemUncommonBrush");
    public static SolidColorBrush LightGreenBrush => Get<SolidColorBrush>("LightGreenBrush");
    public static SolidColorBrush MainBrush => Get<SolidColorBrush>("MainBrush");
    public static SolidColorBrush MarksBrush => Get<SolidColorBrush>("MarksBrush");
    public static SolidColorBrush MaxRunemarkBrush => Get<SolidColorBrush>("MaxRunemarkBrush");
    public static SolidColorBrush MoongourdAccentBrush => Get<SolidColorBrush>("MoongourdAccentBrush");
    public static SolidColorBrush MoongourdBackgroundBrush => Get<SolidColorBrush>("MoongourdBackgroundBrush");
    public static SolidColorBrush MoongourdDarkBackgroundBrush => Get<SolidColorBrush>("MoongourdDarkBackgroundBrush");
    public static SolidColorBrush MoongourdFadedAccentBrush => Get<SolidColorBrush>("MoongourdFadedAccentBrush");
    public static SolidColorBrush MoongourdHeaderBrush => Get<SolidColorBrush>("MoongourdHeaderBrush");
    public static SolidColorBrush MpBrush => Get<SolidColorBrush>("MpBrush");
    public static SolidColorBrush MpBrushLight => Get<SolidColorBrush>("MpBrushLight");
    public static SolidColorBrush PreCooldownArcBrush => Get<SolidColorBrush>("PreCooldownArcBrush");
    public static SolidColorBrush RageBrush => Get<SolidColorBrush>("RageBrush");
    public static SolidColorBrush RageBrushDark => Get<SolidColorBrush>("RageBrushDark");
    public static SolidColorBrush RageBrushLight => Get<SolidColorBrush>("RageBrushLight");
    public static SolidColorBrush RevampBackground70Brush => Get<SolidColorBrush>("RevampBackground70Brush");
    public static SolidColorBrush RevampBackground80Brush => Get<SolidColorBrush>("RevampBackground80Brush");
    public static SolidColorBrush RevampBackground90Brush => Get<SolidColorBrush>("RevampBackground90Brush");
    public static SolidColorBrush RevampBackgroundBrush => Get<SolidColorBrush>("RevampBackgroundBrush");
    public static SolidColorBrush RevampBorderBrush => Get<SolidColorBrush>("RevampBorderBrush");
    public static SolidColorBrush RevampDarkBackground70Brush => Get<SolidColorBrush>("RevampDarkBackground70Brush");
    public static SolidColorBrush RevampDarkBackgroundBrush => Get<SolidColorBrush>("RevampDarkBackgroundBrush");
    public static SolidColorBrush RevampDarkerBackground70Brush => Get<SolidColorBrush>("RevampDarkerBackground70Brush");
    public static SolidColorBrush RevampDarkerBackgroundBrush => Get<SolidColorBrush>("RevampDarkerBackgroundBrush");
    public static SolidColorBrush RunemarkBrush => Get<SolidColorBrush>("RunemarkBrush");
    public static SolidColorBrush SilverBrush => Get<SolidColorBrush>("SilverBrush");
    public static SolidColorBrush SkillStrokeBrush => Get<SolidColorBrush>("SkillStrokeBrush");
    public static SolidColorBrush SniperEyeBrush => Get<SolidColorBrush>("SniperEyeBrush");
    public static SolidColorBrush TankRoleBrush => Get<SolidColorBrush>("TankRoleBrush");
    public static SolidColorBrush TccNormalGradient0Brush => Get<SolidColorBrush>("TccNormalGradient0Brush");
    public static SolidColorBrush TccNormalGradient1Brush => Get<SolidColorBrush>("TccNormalGradient1Brush");
    public static SolidColorBrush TccNormalLightGradient0Brush => Get<SolidColorBrush>("TccNormalLightGradient0Brush");
    public static SolidColorBrush TccNormalLightGradient1Brush => Get<SolidColorBrush>("TccNormalLightGradient1Brush");
    public static SolidColorBrush TccRedGradient0Brush => Get<SolidColorBrush>("TccRedGradient0Brush");
    public static SolidColorBrush TccRedGradient1Brush => Get<SolidColorBrush>("TccRedGradient1Brush");
    public static SolidColorBrush TccYellowGradient0Brush => Get<SolidColorBrush>("TccYellowGradient0Brush");
    public static SolidColorBrush TccYellowGradient1Brush => Get<SolidColorBrush>("TccYellowGradient1Brush");
    public static SolidColorBrush TeraRestyleBorderBrush => Get<SolidColorBrush>("TeraRestyleBorderBrush");
    public static SolidColorBrush TeraRestyleBorderBrushLight => Get<SolidColorBrush>("TeraRestyleBorderBrushLight");
    public static SolidColorBrush TeraRestylePanelBrush => Get<SolidColorBrush>("TeraRestylePanelBrush");
    public static SolidColorBrush TerradraxBrush => Get<SolidColorBrush>("TerradraxBrush");
    public static SolidColorBrush Tier1DungeonBrush => Get<SolidColorBrush>("Tier1DungeonBrush");
    public static SolidColorBrush Tier2DungeonBrush => Get<SolidColorBrush>("Tier2DungeonBrush");
    public static SolidColorBrush Tier3DungeonBrush => Get<SolidColorBrush>("Tier3DungeonBrush");
    public static SolidColorBrush Tier4DungeonBrush => Get<SolidColorBrush>("Tier4DungeonBrush");
    public static SolidColorBrush Tier5DungeonBrush => Get<SolidColorBrush>("Tier5DungeonBrush");
    public static SolidColorBrush ToolboxBrush => Get<SolidColorBrush>("ToolboxBrush");
    public static SolidColorBrush TooltipBrush => Get<SolidColorBrush>("TooltipBrush");
    public static SolidColorBrush TwitchBrush => Get<SolidColorBrush>("TwitchBrush");
    public static SolidColorBrush UmbradraxBrush => Get<SolidColorBrush>("UmbradraxBrush");
    public static SolidColorBrush WeeklyBrush => Get<SolidColorBrush>("WeeklyBrush");
    public static SolidColorBrush WillpowerBrush => Get<SolidColorBrush>("WillpowerBrush");
    public static SolidColorBrush WillpowerBrushLight => Get<SolidColorBrush>("WillpowerBrushLight");
}

// ResourceDictionaries/Colors.xaml
public class Colors : RH
{
    public static Color AbnormalityBuffColor => Get<Color>("AbnormalityBuffColor");
    public static Color AbnormalityDebuffColor => Get<Color>("AbnormalityDebuffColor");
    public static Color AbnormalityDotColor => Get<Color>("AbnormalityDotColor");
    public static Color AbnormalityStunColor => Get<Color>("AbnormalityStunColor");
    public static Color AquadraxColor => Get<Color>("AquadraxColor");
    public static Color ArcaneColor => Get<Color>("ArcaneColor");
    public static Color ArcaneColorLight => Get<Color>("ArcaneColorLight");
    public static Color AssaultStanceColor => Get<Color>("AssaultStanceColor");
    public static Color AssaultStanceColorDark => Get<Color>("AssaultStanceColorDark");
    public static Color AssaultStanceColorLight => Get<Color>("AssaultStanceColorLight");
    public static Color BackgroundDarkColor => Get<Color>("BackgroundDarkColor");
    public static Color CardDarkColor => Get<Color>("CardDarkColor");
    public static Color ChatAreaColor => Get<Color>("ChatAreaColor");
    public static Color ChatChampionLaurelColor => Get<Color>("ChatChampionLaurelColor");
    public static Color ChatDiamondLaurelColor => Get<Color>("ChatDiamondLaurelColor");
    public static Color ChatEmoteColor => Get<Color>("ChatEmoteColor");
    public static Color ChatGlobalColor => Get<Color>("ChatGlobalColor");
    public static Color ChatGreetColor => Get<Color>("ChatGreetColor");
    public static Color ChatGuildAdColor => Get<Color>("ChatGuildAdColor");
    public static Color ChatGuildColor => Get<Color>("ChatGuildColor");
    public static Color ChatMegaphoneColor => Get<Color>("ChatMegaphoneColor");
    public static Color ChatMegaphoneColorDark => Get<Color>("ChatMegaphoneColorDark");
    public static Color ChatPartyColor => Get<Color>("ChatPartyColor");
    public static Color ChatPartyNoticeColor => Get<Color>("ChatPartyNoticeColor");
    public static Color ChatPrivateColor => Get<Color>("ChatPrivateColor");
    public static Color ChatProxyColor => Get<Color>("ChatProxyColor");
    public static Color ChatRaidColor => Get<Color>("ChatRaidColor");
    public static Color ChatRaidNoticeColor => Get<Color>("ChatRaidNoticeColor");
    public static Color ChatSayColor => Get<Color>("ChatSayColor");
    public static Color ChatSystemBargainColor => Get<Color>("ChatSystemBargainColor");
    public static Color ChatSystemContractAlertColor => Get<Color>("ChatSystemContractAlertColor");
    public static Color ChatSystemDeathmatchColor => Get<Color>("ChatSystemDeathmatchColor");
    public static Color ChatSystemErrorColor => Get<Color>("ChatSystemErrorColor");
    public static Color ChatSystemEventColor => Get<Color>("ChatSystemEventColor");
    public static Color ChatSystemExpColor => Get<Color>("ChatSystemExpColor");
    public static Color ChatSystemFriendColor => Get<Color>("ChatSystemFriendColor");
    public static Color ChatSystemGenericColor => Get<Color>("ChatSystemGenericColor");
    public static Color ChatSystemGroupAlertColor => Get<Color>("ChatSystemGroupAlertColor");
    public static Color ChatSystemGroupColor => Get<Color>("ChatSystemGroupColor");
    public static Color ChatSystemLootColor => Get<Color>("ChatSystemLootColor");
    public static Color ChatSystemMoneyColor => Get<Color>("ChatSystemMoneyColor");
    public static Color ChatSystemNotifyColor => Get<Color>("ChatSystemNotifyColor");
    public static Color ChatSystemQuestColor => Get<Color>("ChatSystemQuestColor");
    public static Color ChatSystemWorldBossColor => Get<Color>("ChatSystemWorldBossColor");
    public static Color ChatTradeColor => Get<Color>("ChatTradeColor");
    public static Color ChatWhisperColor => Get<Color>("ChatWhisperColor");
    public static Color ClassGlowColor => Get<Color>("ClassGlowColor");
    public static Color CooldownArcColor => Get<Color>("CooldownArcColor");
    public static Color CopperColor => Get<Color>("CopperColor");
    public static Color DailyColor => Get<Color>("DailyColor");
    public static Color DarkGreenColor => Get<Color>("DarkGreenColor");
    public static Color DefaultBackgroundColor => Get<Color>("DefaultBackgroundColor");
    public static Color DefensiveStanceColor => Get<Color>("DefensiveStanceColor");
    public static Color DefensiveStanceColorDark => Get<Color>("DefensiveStanceColorDark");
    public static Color DefensiveStanceColorLight => Get<Color>("DefensiveStanceColorLight");
    public static Color DpsRoleColor => Get<Color>("DpsRoleColor");
    public static Color EnchantHighColor => Get<Color>("EnchantHighColor");
    public static Color EnchantLowColor => Get<Color>("EnchantLowColor");
    public static Color FireColor => Get<Color>("FireColor");
    public static Color FireColorLight => Get<Color>("FireColorLight");
    public static Color GoldColor => Get<Color>("GoldColor");
    public static Color GreenColor => Get<Color>("GreenColor");
    public static Color GuardianColor => Get<Color>("GuardianColor");
    public static Color HealerRoleColor => Get<Color>("HealerRoleColor");
    public static Color HpColor => Get<Color>("HpColor");
    public static Color HpColorLight => Get<Color>("HpColorLight");
    public static Color HpDebuffColor => Get<Color>("HpDebuffColor");
    public static Color HpDebuffColorLight => Get<Color>("HpDebuffColorLight");
    public static Color IceColor => Get<Color>("IceColor");
    public static Color IceColorLight => Get<Color>("IceColorLight");
    public static Color IgnidraxColor => Get<Color>("IgnidraxColor");
    public static Color ItemCommonColor => Get<Color>("ItemCommonColor");
    public static Color ItemHeroicColor => Get<Color>("ItemHeroicColor");
    public static Color ItemRareColor => Get<Color>("ItemRareColor");
    public static Color ItemSuperiorColor => Get<Color>("ItemSuperiorColor");
    public static Color ItemUncommonColor => Get<Color>("ItemUncommonColor");
    public static Color LightGreenColor => Get<Color>("LightGreenColor");
    public static Color MainColor => Get<Color>("MainColor");
    public static Color MarksColor => Get<Color>("MarksColor");
    public static Color MaxRunemarkColor => Get<Color>("MaxRunemarkColor");
    public static Color MoongourdAccentColor => Get<Color>("MoongourdAccentColor");
    public static Color MoongourdBackgroundColor => Get<Color>("MoongourdBackgroundColor");
    public static Color MoongourdDarkBackgroundColor => Get<Color>("MoongourdDarkBackgroundColor");
    public static Color MoongourdFadedAccentColor => Get<Color>("MoongourdFadedAccentColor");
    public static Color MoongourdHeaderColor => Get<Color>("MoongourdHeaderColor");
    public static Color MpColor => Get<Color>("MpColor");
    public static Color MpColorLight => Get<Color>("MpColorLight");
    public static Color PreCooldownArcColor => Get<Color>("PreCooldownArcColor");
    public static Color RageColor => Get<Color>("RageColor");
    public static Color RageColorDark => Get<Color>("RageColorDark");
    public static Color RageColorLight => Get<Color>("RageColorLight");
    public static Color RevampBackgroundColor => Get<Color>("RevampBackgroundColor");
    public static Color RevampBorderColor => Get<Color>("RevampBorderColor");
    public static Color RevampDarkBackgroundColor => Get<Color>("RevampDarkBackgroundColor");
    public static Color RevampDarkerBackgroundColor => Get<Color>("RevampDarkerBackgroundColor");
    public static Color RunemarkColor => Get<Color>("RunemarkColor");
    public static Color SilverColor => Get<Color>("SilverColor");
    public static Color SkillStrokeColor => Get<Color>("SkillStrokeColor");
    public static Color SniperEyeColor => Get<Color>("SniperEyeColor");
    public static Color TankRoleColor => Get<Color>("TankRoleColor");
    public static Color TccGreenGradient0Color => Get<Color>("TccGreenGradient0Color");
    public static Color TccGreenGradient1Color => Get<Color>("TccGreenGradient1Color");
    public static Color TccNormalGradient0Color => Get<Color>("TccNormalGradient0Color");
    public static Color TccNormalGradient1Color => Get<Color>("TccNormalGradient1Color");
    public static Color TccNormalLightGradient0Color => Get<Color>("TccNormalLightGradient0Color");
    public static Color TccNormalLightGradient1Color => Get<Color>("TccNormalLightGradient1Color");
    public static Color TccRedGradient0Color => Get<Color>("TccRedGradient0Color");
    public static Color TccRedGradient1Color => Get<Color>("TccRedGradient1Color");
    public static Color TccWhiteGradient0Color => Get<Color>("TccWhiteGradient0Color");
    public static Color TccWhiteGradient1Color => Get<Color>("TccWhiteGradient1Color");
    public static Color TccYellowGradient0Color => Get<Color>("TccYellowGradient0Color");
    public static Color TccYellowGradient1Color => Get<Color>("TccYellowGradient1Color");
    public static Color TeraRestyleBorderColor => Get<Color>("TeraRestyleBorderColor");
    public static Color TeraRestyleBorderColorLight => Get<Color>("TeraRestyleBorderColorLight");
    public static Color TeraRestylePanelColor => Get<Color>("TeraRestylePanelColor");
    public static Color TerradraxColor => Get<Color>("TerradraxColor");
    public static Color Tier1DungeonColor => Get<Color>("Tier1DungeonColor");
    public static Color Tier2DungeonColor => Get<Color>("Tier2DungeonColor");
    public static Color Tier3DungeonColor => Get<Color>("Tier3DungeonColor");
    public static Color Tier4DungeonColor => Get<Color>("Tier4DungeonColor");
    public static Color Tier5DungeonColor => Get<Color>("Tier5DungeonColor");
    public static Color ToolboxColor => Get<Color>("ToolboxColor");
    public static Color TooltipBadColor => Get<Color>("TooltipBadColor");
    public static Color TooltipColor => Get<Color>("TooltipColor");
    public static Color TooltipGoodColor => Get<Color>("TooltipGoodColor");
    public static Color TwitchColor => Get<Color>("TwitchColor");
    public static Color UmbradraxColor => Get<Color>("UmbradraxColor");
    public static Color WeeklyColor => Get<Color>("WeeklyColor");
    public static Color WillpowerColor => Get<Color>("WillpowerColor");
    public static Color WillpowerColorLight => Get<Color>("WillpowerColorLight");
}

// ResourceDictionaries/Converters.xaml
public class Converters : RH
{
    public static AbnormalityTypeToColorConverter AbnormalityTypeToStrokeColor => Get<AbnormalityTypeToColorConverter>("AbnormalityTypeToStrokeColor");
    public static AggroTypeToFillConverter AggroTypeToFill => Get<AggroTypeToFillConverter>("AggroTypeToFill");
    public static ChatChannelToColorConverter ChatChannelToColor => Get<ChatChannelToColorConverter>("ChatChannelToColor");
    public static ChatChannelToName ChatChannelToName => Get<ChatChannelToName>("ChatChannelToName");
    public static ClassSvgConverter ClassToSvg => Get<ClassSvgConverter>("ClassToSvg");
    public static ClassToFillConverter ClassToFill => Get<ClassToFillConverter>("ClassToFill");
    public static CooldownWindowModeToTemplateConverter CooldowWindowModeToTemplate => Get<CooldownWindowModeToTemplateConverter>("CooldowWindowModeToTemplate");
    public static DragonIdToColorConverter DragonIdToColor => Get<DragonIdToColorConverter>("DragonIdToColor");
    public static DungeonImageConverter DungeonImageConverter => Get<DungeonImageConverter>("DungeonImageConverter");
    public static EntityIdToClassConverter EntityIdToClass => Get<EntityIdToClassConverter>("EntityIdToClass");
    public static EntityIdToNameConverter EntityIdToName => Get<EntityIdToNameConverter>("EntityIdToName");
    public static EntriesToColor EntriesToColor => Get<EntriesToColor>("EntriesToColor");
    public static GearLevelToColorConverter GearLevelToColor => Get<GearLevelToColorConverter>("GearLevelToColor");
    public static GearLevelToFactorConverter GearLevelToFactor => Get<GearLevelToFactorConverter>("GearLevelToFactor");
    public static GroupSizeToTemplateConverter GroupSizeToTemplate => Get<GroupSizeToTemplateConverter>("GroupSizeToTemplate");
    public static GuardianPointsStringConverter GuardianPointsStringConverter => Get<GuardianPointsStringConverter>("GuardianPointsStringConverter");
    public static HHphaseToEnemyWindowTemplate HarrowholdPhaseToLayout => Get<HHphaseToEnemyWindowTemplate>("HarrowholdPhaseToLayout");
    public static HPbarColorConverter DebuffStatusToHpColor => Get<HPbarColorConverter>("DebuffStatusToHpColor");
    public static HPbarColorConverter2 DebuffStatusToHpColor2 => Get<HPbarColorConverter2>("DebuffStatusToHpColor2");
    public static IconConverter IconNameToPath => Get<IconConverter>("IconNameToPath");
    public static ItemLevelTierToColorConverter IlvlTierToColor => Get<ItemLevelTierToColorConverter>("IlvlTierToColor");
    public static LogoInfoToImageConverter LogoInfoToImage => Get<LogoInfoToImageConverter>("LogoInfoToImage");
    public static MessageTextToIconConverter MessageTextToIcon => Get<MessageTextToIconConverter>("MessageTextToIcon");
    public static MessageTypeToCursor MessageTypeToCursor => Get<MessageTypeToCursor>("MessageTypeToCursor");
    public static MoneyAmountToVisibilityConverter AmountToVisibilityConv => Get<MoneyAmountToVisibilityConverter>("AmountToVisibilityConv");
    public static NotificationTypeToBrush NotificationTypeToBrush => Get<NotificationTypeToBrush>("NotificationTypeToBrush");
    public static PieceToPathConverter PieceToPath => Get<PieceToPathConverter>("PieceToPath");
    public static RaidToColorConverter RaidToColor => Get<RaidToColorConverter>("RaidToColor");
    public static RollToStringConverter RollToString => Get<RollToStringConverter>("RollToString");
    public static StringToFillConverter StringToFill => Get<StringToFillConverter>("StringToFill");
    public static ValueConverterGroup ClassToTransparentFill => Get<ValueConverterGroup>("ClassToTransparentFill");
    public static ValueConverterGroup IlvlTierToTransparentFill => Get<ValueConverterGroup>("IlvlTierToTransparentFill");
    public static ValueConverterGroup IsRaidToTransparentFill => Get<ValueConverterGroup>("IsRaidToTransparentFill");
}

// ResourceDictionaries/DataTemplates.xaml
public class DataTemplates : RH
{
    public static DataTemplate AbnormalityDataTemplate => Get<DataTemplate>("AbnormalityDataTemplate");
    public static DataTemplate ApplicantDt => Get<DataTemplate>("ApplicantDt");
    public static DataTemplate ApplyBody => Get<DataTemplate>("ApplyBody");
    public static DataTemplate BossDataTemplate => Get<DataTemplate>("BossDataTemplate");
    public static DataTemplate BrokerOfferBody => Get<DataTemplate>("BrokerOfferBody");
    public static DataTemplate ClickableTemplate => Get<DataTemplate>("ClickableTemplate");
    public static DataTemplate DefaultAuthorTemplate => Get<DataTemplate>("DefaultAuthorTemplate");
    public static DataTemplate DefaultBody => Get<DataTemplate>("DefaultBody");
    public static DataTemplate DefaultChannelLabelTemplate => Get<DataTemplate>("DefaultChannelLabelTemplate");
    public static DataTemplate DefaultEnemyWindowLayout => Get<DataTemplate>("DefaultEnemyWindowLayout");
    public static DataTemplate DefaultNotificationTemplate => Get<DataTemplate>("DefaultNotificationTemplate");
    public static DataTemplate DragonIndicator => Get<DataTemplate>("DragonIndicator");
    public static DataTemplate EmojiTemplate => Get<DataTemplate>("EmojiTemplate");
    public static DataTemplate EnchantChannelLabelTemplate => Get<DataTemplate>("EnchantChannelLabelTemplate");
    public static DataTemplate EnumDescrDataTemplate => Get<DataTemplate>("EnumDescrDataTemplate");
    public static DataTemplate FixedCooldownTemplate => Get<DataTemplate>("FixedCooldownTemplate");
    public static DataTemplate FixedSkillDataTemplateForConfig => Get<DataTemplate>("FixedSkillDataTemplateForConfig");
    public static DataTemplate GroupAbnormalitySelectorDataTemplate => Get<DataTemplate>("GroupAbnormalitySelectorDataTemplate");
    public static DataTemplate GuildTowerTemplate => Get<DataTemplate>("GuildTowerTemplate");
    public static DataTemplate IconTemplate => Get<DataTemplate>("IconTemplate");
    public static DataTemplate ItemDataTemplate => Get<DataTemplate>("ItemDataTemplate");
    public static DataTemplate LfgBody => Get<DataTemplate>("LfgBody");
    public static DataTemplate LfgTemplate => Get<DataTemplate>("LfgTemplate");
    public static DataTemplate ListingDt => Get<DataTemplate>("ListingDt");
    public static DataTemplate MegaphoneChannelLabelTemplate => Get<DataTemplate>("MegaphoneChannelLabelTemplate");
    public static DataTemplate MessageBodyTemplate => Get<DataTemplate>("MessageBodyTemplate");
    public static DataTemplate MessageHeaderTemplate => Get<DataTemplate>("MessageHeaderTemplate");
    public static DataTemplate MobDataTemplate => Get<DataTemplate>("MobDataTemplate");
    public static DataTemplate MoneyTemplate => Get<DataTemplate>("MoneyTemplate");
    public static DataTemplate MoongourdEncounterDataTemplate => Get<DataTemplate>("MoongourdEncounterDataTemplate");
    public static DataTemplate MyAbnormalitySelectorDataTemplate => Get<DataTemplate>("MyAbnormalitySelectorDataTemplate");
    public static DataTemplate NameClassCharDataTemplate => Get<DataTemplate>("NameClassCharDataTemplate");
    public static DataTemplate NameClassCharDataTemplateWithVM => Get<DataTemplate>("NameClassCharDataTemplateWithVM");
    public static DataTemplate NormalCooldownTemplate => Get<DataTemplate>("NormalCooldownTemplate");
    public static DataTemplate PartyDataTemplate => Get<DataTemplate>("PartyDataTemplate");
    public static DataTemplate Phase1EnemyWindowLayout => Get<DataTemplate>("Phase1EnemyWindowLayout");
    public static DataTemplate Phase2BEnemyWindowLayout => Get<DataTemplate>("Phase2BEnemyWindowLayout");
    public static DataTemplate Phase2EnemyWindowLayout => Get<DataTemplate>("Phase2EnemyWindowLayout");
    public static DataTemplate Phase3EnemyWindowLayout => Get<DataTemplate>("Phase3EnemyWindowLayout");
    public static DataTemplate Phase4EnemyWindowLayout => Get<DataTemplate>("Phase4EnemyWindowLayout");
    public static DataTemplate PlayerDt => Get<DataTemplate>("PlayerDt");
    public static DataTemplate ProgressNotificationTemplate => Get<DataTemplate>("ProgressNotificationTemplate");
    public static DataTemplate RaidDataTemplate => Get<DataTemplate>("RaidDataTemplate");
    public static DataTemplate ReadyCheckIndicator => Get<DataTemplate>("ReadyCheckIndicator");
    public static DataTemplate RoleColumnsGroupLayout => Get<DataTemplate>("RoleColumnsGroupLayout");
    public static DataTemplate RollResultIndicator => Get<DataTemplate>("RollResultIndicator");
    public static DataTemplate RoundAbnormality => Get<DataTemplate>("RoundAbnormality");
    public static DataTemplate RoundBossAbnormality => Get<DataTemplate>("RoundBossAbnormality");
    public static DataTemplate RoundFixedSkill => Get<DataTemplate>("RoundFixedSkill");
    public static DataTemplate RoundNormalSkill => Get<DataTemplate>("RoundNormalSkill");
    public static DataTemplate RoundPartyAbnormality => Get<DataTemplate>("RoundPartyAbnormality");
    public static DataTemplate RoundRaidAbnormality => Get<DataTemplate>("RoundRaidAbnormality");
    public static DataTemplate SimpleChatChannelTemplate => Get<DataTemplate>("SimpleChatChannelTemplate");
    public static DataTemplate SimpleTemplate => Get<DataTemplate>("SimpleTemplate");
    public static DataTemplate SingleColumnGroupLayout => Get<DataTemplate>("SingleColumnGroupLayout");
    public static DataTemplate SkillDataTemplate => Get<DataTemplate>("SkillDataTemplate");
    public static DataTemplate SquareAbnormality => Get<DataTemplate>("SquareAbnormality");
    public static DataTemplate SquareBossAbnormality => Get<DataTemplate>("SquareBossAbnormality");
    public static DataTemplate SquareFixedSkill => Get<DataTemplate>("SquareFixedSkill");
    public static DataTemplate SquareNormalSkill => Get<DataTemplate>("SquareNormalSkill");
    public static DataTemplate SquarePartyAbnormality => Get<DataTemplate>("SquarePartyAbnormality");
    public static DataTemplate SquareRaidAbnormality => Get<DataTemplate>("SquareRaidAbnormality");
    public static DataTemplate SystemAuthorTemplate => Get<DataTemplate>("SystemAuthorTemplate");
    public static DataTemplate TabSettingsTemplate => Get<DataTemplate>("TabSettingsTemplate");
    public static DataTemplate TempListingDt => Get<DataTemplate>("TempListingDt");
    public static DataTemplate ToolboxSystemAuthorTemplate => Get<DataTemplate>("ToolboxSystemAuthorTemplate");
    public static DataTemplate WhisperChannelLabelTemplate => Get<DataTemplate>("WhisperChannelLabelTemplate");
    public static Style PlayerListItemStyle => Get<Style>("PlayerListItemStyle");
}

// ResourceDictionaries/DragablzMaterialDesign.xaml
public class DragablzMaterialDesign : RH
{
    public static ControlTemplate TabablzScrollViewerControlTemplate => Get<ControlTemplate>("TabablzScrollViewerControlTemplate");
    public static ShowDefaultCloseButtonConverter ShowDefaultCloseButtonConverter => Get<ShowDefaultCloseButtonConverter>("ShowDefaultCloseButtonConverter");
    public static Style ChatDragableTabItemStyle => Get<Style>("ChatDragableTabItemStyle");
    public static Style ChatTabablzControlStyle => Get<Style>("ChatTabablzControlStyle");
    public static Style MaterialDesignAddItemCommandButtonStyle => Get<Style>("MaterialDesignAddItemCommandButtonStyle");
    public static Style MaterialDesignCloseItemCommandButtonStyle => Get<Style>("MaterialDesignCloseItemCommandButtonStyle");
    public static Style MaterialDesignDragableTabItemStyle => Get<Style>("MaterialDesignDragableTabItemStyle");
    public static Style MaterialDesignDragableTabItemVerticalStyle => Get<Style>("MaterialDesignDragableTabItemVerticalStyle");
    public static Style MaterialDesignFocusVisual => Get<Style>("MaterialDesignFocusVisual");
    public static Style MaterialDesignInvisibleThumbStyle => Get<Style>("MaterialDesignInvisibleThumbStyle");
    public static Style MaterialDesignMenuCommandButtonStyle => Get<Style>("MaterialDesignMenuCommandButtonStyle");
    public static Style MaterialDesignTabablzControlStyle => Get<Style>("MaterialDesignTabablzControlStyle");
    public static Style StandardEmbeddedButtonStyle => Get<Style>("StandardEmbeddedButtonStyle");
    public static Style TabablzDragablzItemsControlStyle => Get<Style>("TabablzDragablzItemsControlStyle");
}

// ResourceDictionaries/MiscResources.xaml
public class MiscResources : RH
{
    public static DropShadowEffect ClassIconGlow => Get<DropShadowEffect>("ClassIconGlow");
    public static DropShadowEffect ClassWindowSkillBorderShadow => Get<DropShadowEffect>("ClassWindowSkillBorderShadow");
    public static FontFamily ArialMonoBold => Get<FontFamily>("ArialMonoBold");
    public static FontFamily Frutiger => Get<FontFamily>("Frutiger");
    public static FontFamily Inconsolata => Get<FontFamily>("Inconsolata");
    public static FontFamily NotoSansMed => Get<FontFamily>("NotoSansMed");
    public static ImageSource BlankImage => Get<ImageSource>("BlankImage");
    public static ImageSource BossIcon => Get<ImageSource>("BossIcon");
    public static ImageSource BossIconFull => Get<ImageSource>("BossIconFull");
    public static ImageSource BronzeLaurel => Get<ImageSource>("BronzeLaurel");
    public static ImageSource BronzeLaurelRhomb => Get<ImageSource>("BronzeLaurelRhomb");
    public static ImageSource BronzeLaurelRhombBig => Get<ImageSource>("BronzeLaurelRhombBig");
    public static ImageSource BronzeLaurelRhombBottom => Get<ImageSource>("BronzeLaurelRhombBottom");
    public static ImageSource ChampionLaurel => Get<ImageSource>("ChampionLaurel");
    public static ImageSource ChampionLaurelRhomb => Get<ImageSource>("ChampionLaurelRhomb");
    public static ImageSource ChampionPinkLaurelRhomb => Get<ImageSource>("ChampionPinkLaurelRhomb");
    public static ImageSource ChampionBlackLaurelRhomb => Get<ImageSource>("ChampionBlackLaurelRhomb");
    public static ImageSource RainbowBySnugLaurelRhomb => Get<ImageSource>("RainbowBySnugLaurelRhomb");
    public static ImageSource ChampionLaurelRhombBig => Get<ImageSource>("ChampionLaurelRhombBig");
    public static ImageSource ChampionPinkLaurelRhombBig => Get<ImageSource>("ChampionPinkLaurelRhombBig");
    public static ImageSource ChampionBlackLaurelRhombBig => Get<ImageSource>("ChampionBlackLaurelRhombBig");
    public static ImageSource RainbowBySnugLaurelRhombBig => Get<ImageSource>("RainbowBySnugLaurelRhombBig");
    public static ImageSource ChampionLaurelRhombBottom => Get<ImageSource>("ChampionLaurelRhombBottom");
    public static ImageSource CharWindowBg => Get<ImageSource>("CharWindowBg");
    public static ImageSource CharWindowBgSide2 => Get<ImageSource>("CharWindowBgSide2");
    public static ImageSource CharWindowBgSorc => Get<ImageSource>("CharWindowBgSorc");
    public static ImageSource CharWindowBgTriple => Get<ImageSource>("CharWindowBgTriple");
    public static ImageSource CharWindowFg => Get<ImageSource>("CharWindowFg");
    public static ImageSource DefaultGuildLogo => Get<ImageSource>("DefaultGuildLogo");
    public static ImageSource DiamondLaurel => Get<ImageSource>("DiamondLaurel");
    public static ImageSource DiamondLaurelRhomb => Get<ImageSource>("DiamondLaurelRhomb");
    public static ImageSource DiamondLaurelRhombBig => Get<ImageSource>("DiamondLaurelRhombBig");
    public static ImageSource DiamondLaurelRhombBottom => Get<ImageSource>("DiamondLaurelRhombBottom");
    public static ImageSource GoldLaurel => Get<ImageSource>("GoldLaurel");
    public static ImageSource GoldLaurelRhomb => Get<ImageSource>("GoldLaurelRhomb");
    public static ImageSource GoldLaurelRhombBig => Get<ImageSource>("GoldLaurelRhombBig");
    public static ImageSource GoldLaurelRhombBottom => Get<ImageSource>("GoldLaurelRhombBottom");
    public static ImageSource MobIcon => Get<ImageSource>("MobIcon");
    public static ImageSource MoongourdLogo => Get<ImageSource>("MoongourdLogo");
    public static ImageSource SilverLaurel => Get<ImageSource>("SilverLaurel");
    public static ImageSource SilverLaurelRhomb => Get<ImageSource>("SilverLaurelRhomb");
    public static ImageSource SilverLaurelRhombBig => Get<ImageSource>("SilverLaurelRhombBig");
    public static ImageSource SilverLaurelRhombBottom => Get<ImageSource>("SilverLaurelRhombBottom");
    public static ImageSource SlotNeutralBg => Get<ImageSource>("SlotNeutralBg");
    public static Storyboard Pulse => Get<Storyboard>("Pulse");
    public static Storyboard Warn => Get<Storyboard>("Warn");
}

// ResourceDictionaries/SVG.xaml
public class SVG : RH
{
    public static Geometry SvgArmor => Get<Geometry>("SvgArmor");
    public static Geometry SvgBelt => Get<Geometry>("SvgBelt");
    public static Geometry SvgBuffIcon => Get<Geometry>("SvgBuffIcon");
    public static Geometry SvgCharacterIcon => Get<Geometry>("SvgCharacterIcon");
    public static Geometry SvgCirclet => Get<Geometry>("SvgCirclet");
    public static Geometry SvgClassArcher => Get<Geometry>("SvgClassArcher");
    public static Geometry SvgClassBerserker => Get<Geometry>("SvgClassBerserker");
    public static Geometry SvgClassBrawler => Get<Geometry>("SvgClassBrawler");
    public static Geometry SvgClassCommon => Get<Geometry>("SvgClassCommon");
    public static Geometry SvgClassGunner => Get<Geometry>("SvgClassGunner");
    public static Geometry SvgClassLancer => Get<Geometry>("SvgClassLancer");
    public static Geometry SvgClassMystic => Get<Geometry>("SvgClassMystic");
    public static Geometry SvgClassNinja => Get<Geometry>("SvgClassNinja");
    public static Geometry SvgClassNone => Get<Geometry>("SvgClassNone");
    public static Geometry SvgClassPriest => Get<Geometry>("SvgClassPriest");
    public static Geometry SvgClassReaper => Get<Geometry>("SvgClassReaper");
    public static Geometry SvgClassSlayer => Get<Geometry>("SvgClassSlayer");
    public static Geometry SvgClassSorcerer => Get<Geometry>("SvgClassSorcerer");
    public static Geometry SvgClassValkyrie => Get<Geometry>("SvgClassValkyrie");
    public static Geometry SvgClassWarrior => Get<Geometry>("SvgClassWarrior");
    public static Geometry SvgCooldownIcon => Get<Geometry>("SvgCooldownIcon");
    public static Geometry SvgCrown => Get<Geometry>("SvgCrown");
    public static Geometry SvgEarring => Get<Geometry>("SvgEarring");
    public static Geometry SvgFeet => Get<Geometry>("SvgFeet");
    public static Geometry SvgHands => Get<Geometry>("SvgHands");
    public static Geometry SvgHide => Get<Geometry>("SvgHide");
    public static Geometry SvgNecklace => Get<Geometry>("SvgNecklace");
    public static Geometry SvgNpcIcon => Get<Geometry>("SvgNpcIcon");
    public static Geometry SvgPauseCircle => Get<Geometry>("SvgPauseCircle");
    public static Geometry SvgRing => Get<Geometry>("SvgRing");
    public static Geometry SvgTera => Get<Geometry>("SvgTera");
    public static Geometry SvgWeapon => Get<Geometry>("SvgWeapon");
}

// ResourceDictionaries/Styles.xaml
public class Styles : RH
{
    public static Style BaseSkillArcStyle => Get<Style>("BaseSkillArcStyle");
    public static Style BaseWindowStyle => Get<Style>("BaseWindowStyle");
    public static Style BoundaryBorderStyle => Get<Style>("BoundaryBorderStyle");
    public static Style ChatTabControlStyle => Get<Style>("ChatTabControlStyle");
    public static Style ClassWindowSkillBorder => Get<Style>("ClassWindowSkillBorder");
    public static Style CooldownSecondsTextStyle => Get<Style>("CooldownSecondsTextStyle");
    public static Style DefaultBorderStyle => Get<Style>("DefaultBorderStyle");
    public static Style DefaultTextStyle => Get<Style>("DefaultTextStyle");
    public static Style GlowHoverGrid => Get<Style>("GlowHoverGrid");
    public static Style GlowHoverGridLeft => Get<Style>("GlowHoverGridLeft");
    public static Style NotificationBody => Get<Style>("NotificationBody");
    public static Style NotificationRectangle => Get<Style>("NotificationRectangle");
    public static Style NotificationTimestamp => Get<Style>("NotificationTimestamp");
    public static Style NotificationTitle => Get<Style>("NotificationTitle");
    public static Style NotificationVersion => Get<Style>("NotificationVersion");
    public static Style RevampBorderStyle => Get<Style>("RevampBorderStyle");
    public static Style ReversedScrollBar => Get<Style>("ReversedScrollBar");
    public static Style RhombSkillArcStyle => Get<Style>("RhombSkillArcStyle");
    public static Style RoundSkillArcStyle => Get<Style>("RoundSkillArcStyle");
    public static Style ScrollThumbs => Get<Style>("ScrollThumbs");
    public static Style ScrollViewerWithReversedVerticalScrollBar => Get<Style>("ScrollViewerWithReversedVerticalScrollBar");
    public static Style SettingsButton => Get<Style>("SettingsButton");
    public static Style SkillDeleteButtonStyle => Get<Style>("SkillDeleteButtonStyle");
    public static Style SkillResetShapeStyle => Get<Style>("SkillResetShapeStyle");
    public static Style SquareSkillArcStyle => Get<Style>("SquareSkillArcStyle");
    public static Style WindowLogo => Get<Style>("WindowLogo");
}

// ResourceDictionaries/TemplateSelectors.xaml
public class TemplateSelectors : RH
{
    public static AbnormalityTemplateSelector BossAbnormalityTemplateSelector => Get<AbnormalityTemplateSelector>("BossAbnormalityTemplateSelector");
    public static AbnormalityTemplateSelector PartyAbnormalityTemplateSelector => Get<AbnormalityTemplateSelector>("PartyAbnormalityTemplateSelector");
    public static AbnormalityTemplateSelector PlayerAbnormalityTemplateSelector => Get<AbnormalityTemplateSelector>("PlayerAbnormalityTemplateSelector");
    public static AbnormalityTemplateSelector RaidAbnormalityTemplateSelector => Get<AbnormalityTemplateSelector>("RaidAbnormalityTemplateSelector");
    public static AuthorLabelTemplateSelector AuthorTemplateSelector => Get<AuthorLabelTemplateSelector>("AuthorTemplateSelector");
    public static ChannelLabelDataTemplateSelector ChannelLabelTemplateSelector => Get<ChannelLabelDataTemplateSelector>("ChannelLabelTemplateSelector");
    public static EnemyTemplateSelector EnemyTemplateSelector => Get<EnemyTemplateSelector>("EnemyTemplateSelector");
    public static GroupWindowTemplateSelector GroupWindowTemplateSelector => Get<GroupWindowTemplateSelector>("GroupWindowTemplateSelector");
    public static ListingTemplateSelector ListingTemplateSelector => Get<ListingTemplateSelector>("ListingTemplateSelector");
    public static MessageBodyDataTemplateSelector BodyTemplateSelector => Get<MessageBodyDataTemplateSelector>("BodyTemplateSelector");
    public static MessagePieceDataTemplateSelector PieceTemplateSelector => Get<MessagePieceDataTemplateSelector>("PieceTemplateSelector");
    public static NotificationTemplateSelector NotificationTemplateSelector => Get<NotificationTemplateSelector>("NotificationTemplateSelector");
    public static SkillTemplateSelector FixedSkillTemplateSelector => Get<SkillTemplateSelector>("FixedSkillTemplateSelector");
    public static SkillTemplateSelector NormalSkillTemplateSelector => Get<SkillTemplateSelector>("NormalSkillTemplateSelector");
}

// pack://application:,,,/Nostrum.WPF;component/Resources/Converters.xaml
public class Nostrum_WPF_Converters : RH
{
    public static BooleanInverter BoolInverter => Get<BooleanInverter>("BoolInverter");
    public static BoolToVisibility BoolToVisibility => Get<BoolToVisibility>("BoolToVisibility");
    public static ColorToTransparent ColorToTransparent => Get<ColorToTransparent>("ColorToTransparent");
    public static DurationToStringConverter DurationToString => Get<DurationToStringConverter>("DurationToString");
    public static EnumDescriptionConverter EnumDescriptionConverter => Get<EnumDescriptionConverter>("EnumDescriptionConverter");
    public static EpochConverter EpochConverter => Get<EpochConverter>("EpochConverter");
    public static FactorToAngleConverter FactorToAngle => Get<FactorToAngleConverter>("FactorToAngle");
    public static ListBoxItemIndexConverter ListBoxItemIndex => Get<ListBoxItemIndexConverter>("ListBoxItemIndex");
    public static MathMultiplicationConverter MathMultiplication => Get<MathMultiplicationConverter>("MathMultiplication");
    public static NullToVisibilityConverter NullToVisibility => Get<NullToVisibilityConverter>("NullToVisibility");
    public static RoundedClipConverter RoundedClipConverter => Get<RoundedClipConverter>("RoundedClipConverter");
    public static ValueToFactorConverter ValueToFactor => Get<ValueToFactorConverter>("ValueToFactor");
}

// pack://application:,,,/Nostrum.WPF;component/Resources/MiscResources.xaml
public class Nostrum_WPF_MiscResources : RH
{
    public static QuadraticEase QuadraticEase => Get<QuadraticEase>("QuadraticEase");
    public static RotateTransform DefaultRotateTransform => Get<RotateTransform>("DefaultRotateTransform");
    public static RotateTransform Rotate45 => Get<RotateTransform>("Rotate45");
    public static RotateTransform Rotate45Inv => Get<RotateTransform>("Rotate45Inv");
    public static ScaleTransform DefaultScaleTransform => Get<ScaleTransform>("DefaultScaleTransform");
    public static SkewTransform Skew45 => Get<SkewTransform>("Skew45");
    public static SkewTransform Skew45Inv => Get<SkewTransform>("Skew45Inv");
    public static TranslateTransform DefaultTranslateTransform => Get<TranslateTransform>("DefaultTranslateTransform");
}

// pack://application:,,,/Nostrum.WPF;component/Resources/SVG.xaml
public class Nostrum_WPF_SVG : RH
{
    public static StreamGeometry SvgAchievements => Get<StreamGeometry>("SvgAchievements");
    public static StreamGeometry SvgAdd => Get<StreamGeometry>("SvgAdd");
    public static StreamGeometry SvgAddCircle => Get<StreamGeometry>("SvgAddCircle");
    public static StreamGeometry SvgAddUser => Get<StreamGeometry>("SvgAddUser");
    public static StreamGeometry SvgAddUsers => Get<StreamGeometry>("SvgAddUsers");
    public static StreamGeometry SvgAuto => Get<StreamGeometry>("SvgAuto");
    public static StreamGeometry SvgAwaken => Get<StreamGeometry>("SvgAwaken");
    public static StreamGeometry SvgBasket => Get<StreamGeometry>("SvgBasket");
    public static StreamGeometry SvgBlock => Get<StreamGeometry>("SvgBlock");
    public static StreamGeometry SvgBlurOff => Get<StreamGeometry>("SvgBlurOff");
    public static StreamGeometry SvgBlurOn => Get<StreamGeometry>("SvgBlurOn");
    public static StreamGeometry SvgChatMessage => Get<StreamGeometry>("SvgChatMessage");
    public static StreamGeometry SvgCheckAll => Get<StreamGeometry>("SvgCheckAll");
    public static StreamGeometry SvgCheckCircle => Get<StreamGeometry>("SvgCheckCircle");
    public static StreamGeometry SvgClose => Get<StreamGeometry>("SvgClose");
    public static StreamGeometry SvgCollapseAll => Get<StreamGeometry>("SvgCollapseAll");
    public static StreamGeometry SvgConfirm => Get<StreamGeometry>("SvgConfirm");
    public static StreamGeometry SvgCopy => Get<StreamGeometry>("SvgCopy");
    public static StreamGeometry SvgCrossedSwords => Get<StreamGeometry>("SvgCrossedSwords");
    public static StreamGeometry SvgDelegateLeader => Get<StreamGeometry>("SvgDelegateLeader");
    public static StreamGeometry SvgDisband => Get<StreamGeometry>("SvgDisband");
    public static StreamGeometry SvgDiscord => Get<StreamGeometry>("SvgDiscord");
    public static StreamGeometry SvgDotsVertical => Get<StreamGeometry>("SvgDotsVertical");
    public static StreamGeometry SvgDownArrow => Get<StreamGeometry>("SvgDownArrow");
    public static StreamGeometry SvgDrag => Get<StreamGeometry>("SvgDrag");
    public static StreamGeometry SvgExpandAll => Get<StreamGeometry>("SvgExpandAll");
    public static StreamGeometry SvgEye => Get<StreamGeometry>("SvgEye");
    public static StreamGeometry SvgFolder => Get<StreamGeometry>("SvgFolder");
    public static StreamGeometry SvgGift => Get<StreamGeometry>("SvgGift");
    public static StreamGeometry SvgGitHub => Get<StreamGeometry>("SvgGitHub");
    public static StreamGeometry SvgGuild => Get<StreamGeometry>("SvgGuild");
    public static StreamGeometry SvgHeart => Get<StreamGeometry>("SvgHeart");
    public static StreamGeometry SvgHide => Get<StreamGeometry>("SvgHide");
    public static StreamGeometry SvgInfo => Get<StreamGeometry>("SvgInfo");
    public static StreamGeometry SvgMail => Get<StreamGeometry>("SvgMail");
    public static StreamGeometry SvgMapMarker => Get<StreamGeometry>("SvgMapMarker");
    public static StreamGeometry SvgMatching => Get<StreamGeometry>("SvgMatching");
    public static StreamGeometry SvgMenuRight => Get<StreamGeometry>("SvgMenuRight");
    public static StreamGeometry SvgMinimize => Get<StreamGeometry>("SvgMinimize");
    public static StreamGeometry SvgMoney => Get<StreamGeometry>("SvgMoney");
    public static StreamGeometry SvgMove => Get<StreamGeometry>("SvgMove");
    public static StreamGeometry SvgOpenLink => Get<StreamGeometry>("SvgOpenLink");
    public static StreamGeometry SvgPaypal => Get<StreamGeometry>("SvgPaypal");
    public static StreamGeometry SvgPen => Get<StreamGeometry>("SvgPen");
    public static StreamGeometry SvgPin => Get<StreamGeometry>("SvgPin");
    public static StreamGeometry SvgQuestionMark => Get<StreamGeometry>("SvgQuestionMark");
    public static StreamGeometry SvgQuestLog => Get<StreamGeometry>("SvgQuestLog");
    public static StreamGeometry SvgReload => Get<StreamGeometry>("SvgReload");
    public static StreamGeometry SvgRemoveCircle => Get<StreamGeometry>("SvgRemoveCircle");
    public static StreamGeometry SvgRemoveUser => Get<StreamGeometry>("SvgRemoveUser");
    public static StreamGeometry SvgSearch => Get<StreamGeometry>("SvgSearch");
    public static StreamGeometry SvgSettings => Get<StreamGeometry>("SvgSettings");
    public static StreamGeometry SvgShare => Get<StreamGeometry>("SvgShare");
    public static StreamGeometry SvgShirt => Get<StreamGeometry>("SvgShirt");
    public static StreamGeometry SvgShop => Get<StreamGeometry>("SvgShop");
    public static StreamGeometry SvgSocial => Get<StreamGeometry>("SvgSocial");
    public static StreamGeometry SvgStar => Get<StreamGeometry>("SvgStar");
    public static StreamGeometry SvgTrophy => Get<StreamGeometry>("SvgTrophy");
    public static StreamGeometry SvgTwitch => Get<StreamGeometry>("SvgTwitch");
    public static StreamGeometry SvgUnpin => Get<StreamGeometry>("SvgUnpin");
    public static StreamGeometry SvgUpArrow => Get<StreamGeometry>("SvgUpArrow");
    public static StreamGeometry SvgUser => Get<StreamGeometry>("SvgUser");
    public static StreamGeometry SvgUserAdd => Get<StreamGeometry>("SvgUserAdd");
    public static StreamGeometry SvgUsers => Get<StreamGeometry>("SvgUsers");
    public static StreamGeometry SvgWarning => Get<StreamGeometry>("SvgWarning");
}

// pack://application:,,,/Nostrum.WPF;component/Resources/Styles.xaml
public class Nostrum_WPF_Styles : RH
{
    public static ControlTemplate ComboBoxEditableTemplate => Get<ControlTemplate>("ComboBoxEditableTemplate");
    public static ControlTemplate ComboBoxTemplate => Get<ControlTemplate>("ComboBoxTemplate");
    public static ControlTemplate MenuItemControlTemplate1 => Get<ControlTemplate>("MenuItemControlTemplate1");
    public static DropShadowEffect BigDropShadow => Get<DropShadowEffect>("BigDropShadow");
    public static DropShadowEffect DropShadow => Get<DropShadowEffect>("DropShadow");
    public static DropShadowEffect FadedDropShadow => Get<DropShadowEffect>("FadedDropShadow");
    public static SolidColorBrush SelectionBackgroundBrush => Get<SolidColorBrush>("SelectionBackgroundBrush");
    public static SolidColorBrush SelectionBackgroundLightBrush => Get<SolidColorBrush>("SelectionBackgroundLightBrush");
    public static SolidColorBrush SelectionBorderBrush => Get<SolidColorBrush>("SelectionBorderBrush");
    public static Style ButtonContentOpacityStyle => Get<Style>("ButtonContentOpacityStyle");
    public static Style NostrumMainButtonStyle => Get<Style>("NostrumMainButtonStyle");
    public static Style ComboBoxEditableTextBox => Get<Style>("ComboBoxEditableTextBox");
    public static Style ComboBoxMainStyle => Get<Style>("ComboBoxMainStyle");
    public static Style ComboBoxToggleButton => Get<Style>("ComboBoxToggleButton");
    public static Style DefaultListItemStyle => Get<Style>("DefaultListItemStyle");
    public static Style EmptyFocusVisual => Get<Style>("EmptyFocusVisual");
    public static Style NoHilightListItemStyle => Get<Style>("NoHilightListItemStyle");
    public static Style NoHilightListItemStyleWithLines => Get<Style>("NoHilightListItemStyleWithLines");
}

public class RH
{
    protected static T Get<T>(string res)
    {
        return (T)Application.Current.FindResource(res);
    }
}