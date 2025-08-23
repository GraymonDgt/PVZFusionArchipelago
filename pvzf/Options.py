import typing
from dataclasses import dataclass
from Options import DefaultOnToggle, Range, Toggle, DeathLink, Choice, PerGameCommonOptions, OptionSet, OptionGroup



class CompletionType(Choice):
    """Set goal for victory
    Zomboss - complete the Dr. Zomboss' Revenge mini-game (unlocks with roof)
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


class FusionChallengeSanity(Toggle):
    """Adds the 11 fusion challenge levels as an item/locations"""
    display_name = "Enable Fusion Challenges"

class FusionShowcaseSanity(Toggle):
    """Adds the 12 fusion showcase levels as an item/locations"""
    display_name = "Enable Fusion Showcase"

class MinigameSanity(Toggle):
    """Adds the 44 normal mini-games as items/locations (PLACEHOLDER NOT IMPLEMENTED)"""
    display_name = "Enable Mini-games"

class SeedSlots(DefaultOnToggle):
    """Randomize seed slots"""
    display_name = "Seed Slots"

class TrapPercentage(Range):
    """Percentage of filler items to replace with traps"""
    display_name = "Trap Percentage"
    range_start = 0
    range_end = 100
    default = 0

class RingLink(Choice):
    """Enable Ringlink (share Sun with other games' currency)
    Normal - Sun resets locally between levels"""
    option_off = 0
    option_normal = 1



pvzf_options_groups = [
OptionGroup("Location Toggles", [
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
    RingLink
])
]

@dataclass
class PVZFOptions(PerGameCommonOptions):
    goal_type: CompletionType
    showcase_sanity: FusionShowcaseSanity
    challenge_sanity: FusionChallengeSanity
    minigame_sanity: MinigameSanity
    randomize_seed_slots: SeedSlots
    trap_percentage: TrapPercentage
    ring_link: RingLink
    death_link: DeathLink

