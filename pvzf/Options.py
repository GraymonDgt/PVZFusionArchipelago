import typing
from dataclasses import dataclass
from Options import DefaultOnToggle, Range, Toggle, DeathLink, Choice, PerGameCommonOptions, OptionSet, OptionGroup




class TrapPercentage(Range):
    """Percentage of filler items to replace with traps"""
    display_name = "Trap Percentage"
    range_start = 0
    range_end = 100
    default = 0




pvzf_options_groups = [
#    OptionGroup("Emblem Toggles", [
#        TimeEmblems,
#        RingEmblems,
#        ScoreEmblems,
#        RankEmblems,
#        NTimeEmblems,
#        StartingCharacter,
#        OneUpSanity,
#        SuperRingSanity,
#        MPMaps
#    ]),
OptionGroup("Meta Options", [
    TrapPercentage
])
]

@dataclass
class PVZFOptions(PerGameCommonOptions):
    trap_percentage:TrapPercentage
    death_link: DeathLink

