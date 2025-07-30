from typing import NamedTuple

from BaseClasses import Item, ItemClassification


class PVZFItem(Item):
    game: str = "Plants Vs Zombies Fusion"


class PVZFItemData(NamedTuple):
    code: int | None = None
    classification: ItemClassification = ItemClassification.progression


generic_item_data_table: dict[str, PVZFItemData] = {
    "Seed Slot": PVZFItemData(201, ItemClassification.filler),
    "Bonus Sun": PVZFItemData(200, ItemClassification.filler),
    #"Lawn Clear":PVZFItemData(201, ItemClassification.filler),
    #refreshed cooldowns
}
traps_item_data_table:dict[str, PVZFItemData] = {
"More Zombies": PVZFItemData(220, ItemClassification.trap),
#Queen Jack in the box
#Sun loss
#seed packet shuffle
    # lawn shuffle
#bungee ambush
#grave danger
#lawn clear
}

access_item_table: dict[str, PVZFItemData] = {
    "Day Access": PVZFItemData(90, ItemClassification.progression),
    "Night Access": PVZFItemData(91, ItemClassification.progression),
    "Pool Access": PVZFItemData(92, ItemClassification.progression),
    "Fog Access": PVZFItemData(93, ItemClassification.progression),
    "Roof Access": PVZFItemData(94, ItemClassification.progression),
    "Snow Access": PVZFItemData(95, ItemClassification.progression),
    #"Fusion Challenges": PVZFItemData(96, ItemClassification.progression),
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
    "Grave Buster": PVZFItemData(18, ItemClassification.progression),
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
    "Platern": PVZFItemData(29, ItemClassification.progression),
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
    "Electronion": PVZFItemData(61, ItemClassification.progression),
    "Sniper Pea": PVZFItemData(62, ItemClassification.progression),#odyssey exclusive
    "Chrysanctum": PVZFItemData(63, ItemClassification.progression),
    "Icetip Lily": PVZFItemData(64, ItemClassification.progression),
    "Pearmafrost": PVZFItemData(65, ItemClassification.progression),
    "Doubleblast Passionfruit": PVZFItemData(66, ItemClassification.progression),

}

tools_item_data_table: dict[str, PVZFItemData] = {
    "Shovel": PVZFItemData(70, ItemClassification.useful),
    #"Fertilizer": PVZFItemData(71, ItemClassification.progression),
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






item_data_table = {
    **generic_item_data_table,**traps_item_data_table,**plants_item_data_table,**tools_item_data_table,**access_item_table
}

item_table = {name: data.code for name, data in item_data_table.items() if data.code is not None}
