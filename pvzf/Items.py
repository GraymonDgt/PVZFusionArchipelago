from typing import NamedTuple

from BaseClasses import Item, ItemClassification


class PVZFItem(Item):
    game: str = "Plants Vs Zombies Fusion"


class PVZFItemData(NamedTuple):
    code: int | None = None
    classification: ItemClassification = ItemClassification.progression


generic_item_data_table: dict[str, PVZFItemData] = {
    "Seed Slot": PVZFItemData(201, ItemClassification.progression),
    "Bonus Sun": PVZFItemData(200, ItemClassification.filler),
    #"Lawn Clear":PVZFItemData(201, ItemClassification.filler),
    #refreshed cooldowns
}
traps_item_data_table:dict[str, PVZFItemData] = {
"Placebo Trap": PVZFItemData(220, ItemClassification.trap),
#Queen Jack in the box
#Sun loss
#seed packet shuffle
    # lawn shuffle
#bungee ambush
#grave danger
#lawn clear
#play a minigame
#rough economy - doubles all plant costs
#conveyor belt trap
}

access_item_table: dict[str, PVZFItemData] = {
    "Day Access": PVZFItemData(90, ItemClassification.progression),
    "Night Access": PVZFItemData(91, ItemClassification.progression),
    "Pool Access": PVZFItemData(92, ItemClassification.progression),
    "Fog Access": PVZFItemData(93, ItemClassification.progression),
    "Roof Access": PVZFItemData(94, ItemClassification.progression),
    "Snow Access": PVZFItemData(95, ItemClassification.progression),
    "Fusion Challenge Access": PVZFItemData(96, ItemClassification.progression),
    "Fusion Showcase Access": PVZFItemData(97, ItemClassification.progression),
    "Odyssey Mode": PVZFItemData(98, ItemClassification.progression),
    "Abyss Mode": PVZFItemData(99, ItemClassification.progression),
}


plants_item_data_table: dict[str, PVZFItemData] = {
    "Peashooter": PVZFItemData(1, ItemClassification.progression),
    "Sunflower":PVZFItemData(2, ItemClassification.progression),
    "Cherry Bomb":PVZFItemData(3, ItemClassification.progression),
    "Wall-nut": PVZFItemData(4, ItemClassification.progression),
    "Potato Mine": PVZFItemData(5, ItemClassification.progression),
    "Chomper": PVZFItemData(6, ItemClassification.progression),
    "Plant Giftbox": PVZFItemData(7, ItemClassification.useful),
    "Tall-nut": PVZFItemData(8, ItemClassification.progression),
    "EndoFlame": PVZFItemData(9, ItemClassification.useful),
    "Puff-shroom": PVZFItemData(10, ItemClassification.progression),
    "Fume-shroom": PVZFItemData(11, ItemClassification.progression),
    "Hypno-shroom": PVZFItemData(12, ItemClassification.progression),
    "Scaredy-shroom": PVZFItemData(13, ItemClassification.progression),
    "Ice-shroom": PVZFItemData(14, ItemClassification.progression),
    "Doom-shroom": PVZFItemData(15, ItemClassification.progression),
    "Zombie Giftbox": PVZFItemData(16, ItemClassification.useful),
    "Gloom-shroom": PVZFItemData(17, ItemClassification.progression),
    "Grave Buster": PVZFItemData(18, ItemClassification.useful),
    "Lily Pad": PVZFItemData(19, ItemClassification.progression),
    "Squash": PVZFItemData(20, ItemClassification.progression),
    "Threepeater": PVZFItemData(21, ItemClassification.progression),
    "Tangle Kelp": PVZFItemData(22, ItemClassification.progression),
    "Jalapeno": PVZFItemData(23, ItemClassification.progression),
    "Spikeweed": PVZFItemData(24, ItemClassification.progression),
    "Torchwood": PVZFItemData(25, ItemClassification.progression),
    "Spikerock": PVZFItemData(26, ItemClassification.progression),
    "Barley": PVZFItemData(27, ItemClassification.useful),
    "Sea-shroom": PVZFItemData(28, ItemClassification.progression),
    "Plantern": PVZFItemData(29, ItemClassification.progression),
    "Cactus": PVZFItemData(30, ItemClassification.progression),
    "Blover": PVZFItemData(31, ItemClassification.progression),
    "Starfruit": PVZFItemData(32, ItemClassification.progression),
    "Pumpkin": PVZFItemData(33, ItemClassification.progression),
    "Magnet-shroom": PVZFItemData(34, ItemClassification.progression),
    "Cattail": PVZFItemData(35, ItemClassification.progression),
    "Imitater": PVZFItemData(36, ItemClassification.useful),
    "Cabbage-pult": PVZFItemData(37, ItemClassification.progression),
    "Flower Pot": PVZFItemData(38, ItemClassification.progression),
    "Kernel-pult": PVZFItemData(39, ItemClassification.progression),
    "Garlic": PVZFItemData(40, ItemClassification.progression),
    "Umbrella Leaf": PVZFItemData(41, ItemClassification.progression),
    "Marigold": PVZFItemData(42, ItemClassification.progression),
    "Melon-pult": PVZFItemData(43, ItemClassification.progression),
    "Cob Cannon": PVZFItemData(44, ItemClassification.progression),
    "Jicamagic": PVZFItemData(45, ItemClassification.progression),
    "Firnace": PVZFItemData(46, ItemClassification.progression),
    "Spruce Sharpshooter": PVZFItemData(47, ItemClassification.progression),
    "Saw-me-not": PVZFItemData(48, ItemClassification.progression),
    "Snow Lotus": PVZFItemData(49, ItemClassification.progression),
    "Aloe Aqua": PVZFItemData(50, ItemClassification.progression),
    "Bamblock": PVZFItemData(51, ItemClassification.progression),
    "Frozen Giftbox": PVZFItemData(52, ItemClassification.useful),
    "Spruce Ballista": PVZFItemData(53, ItemClassification.progression),
    #"???": PVZFItemData(54, ItemClassification.filler),

    "Cattail Girl": PVZFItemData(55, ItemClassification.progression),
    "Swordmaster Starfruit": PVZFItemData(56, ItemClassification.progression),
    "Neko Squash": PVZFItemData(57, ItemClassification.progression),
    "Burger Blaster": PVZFItemData(58, ItemClassification.progression),
    "Queen Endoflame": PVZFItemData(59, ItemClassification.progression),#odyssey exclusive
    "Coldsnap Bean": PVZFItemData(60, ItemClassification.progression),
    "Amp-nion": PVZFItemData(61, ItemClassification.progression),
    "Sniper Pea": PVZFItemData(62, ItemClassification.progression),#odyssey exclusive
    "Chrysanctum": PVZFItemData(63, ItemClassification.progression),
    "Icetip Lily": PVZFItemData(64, ItemClassification.progression),
    "Pearmafrost": PVZFItemData(65, ItemClassification.progression),
    "Doubleblast Passionfruit": PVZFItemData(66, ItemClassification.progression),

}

tools_item_data_table: dict[str, PVZFItemData] = {
    "Shovel": PVZFItemData(70, ItemClassification.useful),
    "Fertilizer": PVZFItemData(71, ItemClassification.progression),
    "Plant Gloves":PVZFItemData(72, ItemClassification.progression),
    #"Zombie Gloves": PVZFItemData(73, ItemClassification.useful),
    "Mallet": PVZFItemData(74, ItemClassification.progression),

    #"Almanac": PVZFItemData(75, ItemClassification.filler),
    "Watering Can": PVZFItemData(76, ItemClassification.filler),
    "Phonograph": PVZFItemData(77, ItemClassification.filler),
    "Bug Spray": PVZFItemData(78, ItemClassification.filler),
    "Wheelbarrow": PVZFItemData(79, ItemClassification.filler),
    #"Advanced Fusions": PVZFItemData(80, ItemClassification.progression),
    #"Lawnmowers": PVZFItemData(159, ItemClassification.progression),
    #"Pool Cleaners": PVZFItemData(160, ItemClassification.progression),




}

minigame_item_table: dict[str, PVZFItemData] = {

    "Scaredy's Dream": PVZFItemData(110, ItemClassification.progression),
    "Pole Vaulting Disco": PVZFItemData(111, ItemClassification.progression),
    "Compact Planting": PVZFItemData(112, ItemClassification.progression),
    "Newspaper War": PVZFItemData(113, ItemClassification.progression),
    "D-Day": PVZFItemData(114, ItemClassification.progression),
    "ZombiesTD": PVZFItemData(115, ItemClassification.progression),
    "ZombiesTD 2": PVZFItemData(116, ItemClassification.progression),
    "Matryoshka": PVZFItemData(117, ItemClassification.progression),
    "Columns Like You See 'Em": PVZFItemData(118, ItemClassification.progression),
    "Mirrors Like You See 'Em": PVZFItemData(119, ItemClassification.progression),
    "It's Raining Seeds": PVZFItemData(120, ItemClassification.progression),
    "Last Stand": PVZFItemData(121, ItemClassification.progression),
    "Air Raid": PVZFItemData(122, ItemClassification.progression),
    "Advanced Challenge: 12-Lane Day": PVZFItemData(123, ItemClassification.progression),
    "Advanced Challenge: 12-Lane Pool": PVZFItemData(124, ItemClassification.progression),
    "ZombiesTD 3": PVZFItemData(125, ItemClassification.progression),

    "True Art is an Explosion!": PVZFItemData(126, ItemClassification.progression),
    "Pogo Party!": PVZFItemData(127, ItemClassification.progression),
    "Attack on Gargantuar!": PVZFItemData(128, ItemClassification.progression),
    "Zum-nut!": PVZFItemData(129, ItemClassification.progression),
    "Squash Showdown!": PVZFItemData(130, ItemClassification.progression),
    "Hypno-tism!": PVZFItemData(131, ItemClassification.progression),
    "Dr Zomboss' Revenge": PVZFItemData(132, ItemClassification.progression),
    "Protect the Gold Magnet": PVZFItemData(133, ItemClassification.progression),
    "Compact Planting 2": PVZFItemData(134, ItemClassification.progression),
    "Bungee Blitz": PVZFItemData(135, ItemClassification.progression),
    "Beghouled": PVZFItemData(136, ItemClassification.progression),
    "Seeing Stars": PVZFItemData(137, ItemClassification.progression),
    "Wall-nut Billiards": PVZFItemData(138, ItemClassification.progression),
    "Wall-nut Billiards 2": PVZFItemData(139, ItemClassification.progression),
    "Wall-nut Billiards 3": PVZFItemData(140, ItemClassification.progression),
    "Whack a Zombie": PVZFItemData(141, ItemClassification.progression),
    "Zombie Nimble Zombie Quick": PVZFItemData(142, ItemClassification.progression),
    "High Gravity": PVZFItemData(143, ItemClassification.progression),

    "Chomper Snake": PVZFItemData(144, ItemClassification.progression),
    "Chinese Chezz": PVZFItemData(145, ItemClassification.progression),
    "Squash Showdown 2": PVZFItemData(146, ItemClassification.progression),
    "Zombies VS Zombies 2": PVZFItemData(147, ItemClassification.progression),#yes this is a normal minigame not odyssey for some reason
    "2048: Peavolution": PVZFItemData(148, ItemClassification.progression),
    "Splash and Clash": PVZFItemData(149, ItemClassification.progression),
    "Melon Ninja": PVZFItemData(150, ItemClassification.progression),
    "Eclipse": PVZFItemData(151, ItemClassification.progression),
    "Wall-nut Bowling": PVZFItemData(152, ItemClassification.progression),
    "Iceborg Executrix's Revenge": PVZFItemData(153, ItemClassification.progression),
}

progressives_item_data_table: dict[str, PVZFItemData] = {

    "Progressive Compact Planting": PVZFItemData(154, ItemClassification.progression),
    "Progressive ZombiesTD": PVZFItemData(155, ItemClassification.progression),
    "Progressive Wall-nut Billiards": PVZFItemData(156, ItemClassification.progression),
    "Progressive Squash Showdown": PVZFItemData(157, ItemClassification.progression),

    #odyssey exclusive
    "Progressive Wack-a-Zombie": PVZFItemData(158, ItemClassification.progression),
    "Progressive Zombies VS Zombies": PVZFItemData(159, ItemClassification.progression),
}


item_data_table = {
    **generic_item_data_table,**traps_item_data_table,**plants_item_data_table,**tools_item_data_table,**access_item_table,**minigame_item_table
}

item_table = {name: data.code for name, data in item_data_table.items() if data.code is not None}
