import typing
from dataclasses import dataclass
from Options import DefaultOnToggle, Range, Toggle, DeathLink, Choice, PerGameCommonOptions, OptionSet, OptionGroup, OptionList



class CompletionType(Choice):
    """Set goal for victory
    Zomboss - complete the Dr. Zomboss' Revenge mini-game (unlocks in the minigames tab with roof)
    All X-9s - Complete every main area's 9th level"""
    option_zomboss = 0
    option_all_x9s = 1
    default = 0
    #Odyssey Adventure - Complete Odyssey Adventure (not implemented)
    #Level Percentage - complete the specified percentage of levels (not implemented)

# when minigame-sanity is off


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
    """Adds the 11 fusion challenge levels as an item/locations"""
    display_name = "Enable Fusion Challenges"

class FusionShowcaseSanity(Toggle):
    """Adds the 88 fusion showcase levels as an item/locations
    CURRENTLY NON-FUNCTIONAL"""
    display_name = "Enable Fusion Showcase"

class MinigameSanity(Choice):
    """Short: Adds 21 shorter 2-flag Minigames as items/locations
    All: Adds all 50 normal mini-games as items/locations"""
    option_off = 0
    option_short = 1
    option_all = 2
    default = 0

    #display_name = "Enable Mini-games"

class SeedSlots(DefaultOnToggle):
    """Randomize seed slots"""
    display_name = "Seed Slots"

class TrapPercentage(Range):
    """Percentage of filler items to replace with traps"""
    display_name = "Trap Percentage"
    range_start = 0
    range_end = 100
    default = 0


class LogicDifficulty(Choice):
    """Logic difficulty
    simple - lily pad required for pool, flower pot required for roof etc.
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
    valid_keys = {"Cattail Girl","Swordmaster Starfruit","Nyan Squash","Burger Blaster","Queen Endoflame","Coldsnap Bean","Amp-nion","Sniper Pea","Chrysanctum","Icetip Lily","Pearmafrost","Doubleblast Passionfruit"}


pvzf_options_groups = [
OptionGroup("Location Toggles", [
        AdventureExtra,
        FusionShowcaseSanity,
        FusionChallengeSanity,
        MinigameSanity
#        StartingCharacter,
#        OneUpSanity,
#        SuperRingSanity,
#        MPMaps
 ]),
OptionGroup("Meta Options", [
    CompletionType,
    SeedSlots,
    TrapPercentage,
    RingLink,
    UniquePlantsPreset,
    StartingPlantBlacklist,
    UniquePlantBlacklist

])
]

@dataclass
class PVZFOptions(PerGameCommonOptions):
    goal_type: CompletionType
    adventure_extra: AdventureExtra
    showcase_sanity: FusionShowcaseSanity
    challenge_sanity: FusionChallengeSanity
    minigame_sanity: MinigameSanity
    randomize_seed_slots: SeedSlots
    trap_percentage: TrapPercentage
    ring_link: RingLink
    death_link: DeathLink
    unique_preset: UniquePlantsPreset
    no_start_plant: StartingPlantBlacklist
    unique_blacklist: UniquePlantBlacklist

