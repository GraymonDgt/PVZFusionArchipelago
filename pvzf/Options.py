import typing
from dataclasses import dataclass
from Options import DefaultOnToggle, Range, Toggle, DeathLink, Choice, PerGameCommonOptions, OptionSet, OptionGroup, OptionList



class CompletionType(Choice):
    """Set goal for victory
    Zomboss - complete the Dr. Zomboss' Revenge mini-game (unlocks in the minigames tab with roof)
    All X-9s - Complete every main area's 9th level
    Odyssey Adventure - Complete Odyssey Adventure: Level 15
    Trophy Hunt - Collect a percentage of trophies"""
    option_zomboss = 0
    option_all_x9s = 1
    option_odyssey_adventure = 2
    option_trophy_hunt = 3
    default = 0
    #Odyssey Adventure - Complete Odyssey Adventure (not implemented)
    #Level Percentage - complete the specified percentage of levels (not implemented)
    #Trophy Collectathon - find the specified number of trophies

# when minigame-sanity is off
class HuntNumber(Range):
    """Number of trophies to be included for trophy hunt"""
    display_name = "Trophy number"
    range_start = 1
    range_end = 100
    default = 10
class HuntPercentage(Range):
    """PERCENTAGE of trophies required to goal for trophy hunt"""
    display_name = "Trophy percentage"
    range_start = 1
    range_end = 100
    default = 50
#class GoalPercentage(Range):
#    """PERCENTAGE of levels needed to goal with the Level percentage goal option
#    THIS IS A PERCENTAGE OUT OF 100, NOT THE NUMBER OF LEVELS"""
#    display_name = "Goal Percentage"
#    range_start = 0
#    range_end = 100
#    default = 0

class AdventureExtra(Choice):
    """Include the Snow levels/plants as items/locations
    shuffle plants - includes the snow plants but not the levels"""
    option_off = 0
    option_shuffle_plants = 1
    option_on = 2
    default = 2

class FusionChallengeSanity(Toggle):
    """Adds the 12 fusion challenge levels as an item/locations"""
    display_name = "Enable Fusion Challenges"

#class FusionShowcaseSanity(Toggle):
#   """Adds the 88 fusion showcase levels as an item/locations
#    CURRENTLY NON-FUNCTIONAL"""
#    display_name = "Enable Fusion Showcase"
class TenFlagSanity(Toggle):
    """Adds the 12 Ten-flag levels as items/locations"""
    display_name = "Enable Ten-flag challenges"

class MinigameSanity(Choice):
    """Short: Adds 22 shorter 2-flag Minigames as items/locations
    All: Adds all 50 normal mini-games as items/locations"""
    option_off = 0
    option_short = 1
    option_all = 2
    default = 0

class VasebreakerSanity(Toggle):
    """Adds the 3 vasebreaker levels as an item/locations"""
    display_name = "Enable Vasebreaker"


class SurvivalSanity(Toggle):
    """Adds the 10 survival mode levels as items/locations"""
    display_name = "Enable Survival Mode"



class AdventureOdyssey(Toggle):
    """Adds the 15 Odyssey Adventure levels into the pool as items/locations"""
    display_name = "Enable Adventure Odyssey"

    #display_name = "Enable Mini-games"

class SeedSlots(DefaultOnToggle):
    """Randomize seed slots"""
    display_name = "Seed Slots"

class LawnCleaners(Toggle):
    """Randomize lawnmowers/pool cleaners"""
    display_name = "Lawnmowers"

class TrapPercentage(Range):
    """Percentage of filler items to replace with traps"""
    display_name = "Trap Percentage"
    range_start = 0
    range_end = 100
    default = 30


class LogicDifficulty(Choice):
    """Logic difficulty
    simple - lily pad required for pool, flower pot required for roof, mushrooms required for night etc.
    normal - pool/roof/snow in logic with the right attackers to deal with everything
    hard - may require tedious/ unreliable/ obscure strategies to beat levels"""
    option_simple = 0
    option_normal = 1
    #option_hard = 2
    default = 1



class RingLink(Choice):
    """Enable Ringlink (share Sun with other games' currency)
    Normal - Sun resets locally between levels"""
    option_off = 0
    option_normal = 1

class StartingPlantBlacklist(OptionList):
    """Blacklist for starting plants
     valid keys: {"Peashooter","Fume-shroom","Scaredy-shroom","Threepeater","Cactus","Starfruit","Cabbage-pult","Kernel-pult","Spruce Sharpshooter"}"""
    display_name = "Starting Plant Blacklist"
    valid_keys = {"Peashooter","Fume-shroom","Scaredy-shroom","Threepeater","Cactus","Starfruit","Cabbage-pult","Kernel-pult","Spruce Sharpshooter"}

class UniquePlantsPreset(Choice):
    """On - all unique plants are in the pool (these tend to be very overpowered so use at your own risk)
    Exclude Overpowered - removes unique plants that are considered overpowered
    None - removes all unique plants from the pool
    """
    option_on = 0
    option_exclude_overpowered = 1
    option_none = 2

class UniquePlantBlacklist(OptionList):
    """Blacklist for unique plants, keys are by almanac entry
     Set Unqiue plants to 'on' and blacklist specific ones here"""
    display_name = "Unique Plant Blacklist"
    valid_keys = {"Cattail Girl","Swordmaster Starfruit","Nyan Squash","Burger Blaster","Queen Endoflame","Coldsnap Bean","Amp-nion","Snipea","Chrysanctum","Icetip Lily","Pearmafrost","Doubleblast Passionfruit","Lucky Blover","Diamond Imitater"}


pvzf_options_groups = [
OptionGroup("Location Toggles", [
        AdventureExtra,
        #FusionShowcaseSanity,
        FusionChallengeSanity,
        #TenFlagSanity,
        MinigameSanity,
        VasebreakerSanity,
        SurvivalSanity,
        AdventureOdyssey
#        StartingCharacter,
#        OneUpSanity,
#        SuperRingSanity,
#        MPMaps
 ]),
OptionGroup("Meta Options", [
    CompletionType,
    HuntNumber,
    HuntPercentage,
    SeedSlots,
    LawnCleaners,
    TrapPercentage,
    LogicDifficulty,
    RingLink,
    UniquePlantsPreset,
    StartingPlantBlacklist,
    UniquePlantBlacklist

])
]

@dataclass
class PVZFOptions(PerGameCommonOptions):
    goal_type: CompletionType
    trophy_number:HuntNumber
    trophy_percentage:HuntPercentage
    adventure_extra: AdventureExtra
    #showcase_sanity: FusionShowcaseSanity
    challenge_sanity: FusionChallengeSanity
    #ten_flag_sanity:TenFlagSanity
    minigame_sanity: MinigameSanity
    vasebreaker_sanity:VasebreakerSanity
    survival_sanity:SurvivalSanity
    adventure_odyssey: AdventureOdyssey
    randomize_seed_slots: SeedSlots
    randomizer_mowers: LawnCleaners
    trap_percentage: TrapPercentage
    logic_difficulty: LogicDifficulty
    ring_link: RingLink
    death_link: DeathLink
    unique_preset: UniquePlantsPreset
    unique_blacklist: UniquePlantBlacklist
    no_start_plant: StartingPlantBlacklist

