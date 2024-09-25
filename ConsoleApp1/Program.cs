namespace ConsoleApp1
{
    internal class Program
    {
        public interface ICharacter
        {
            int Level { get; }
            string Name { get; }
            string Class { get; }
            int Attack { get; }
            int Defense { get; }
            int Hp { get; set; }
            int Gold { get; set; }

            void TakeDamage(int damage);
        }

        //플레이어 정보
        public class Player : ICharacter
        {
            public int Level { get; private set; }
            public string Name { get; private set; }
            public string Class { get; protected set; }
            public int Attack { get; protected set; }
            public int Defense { get; protected set; }
            public int Hp { get; set; }
            public int Gold { get; set; }


            public Player(string name, int level)
            {
                Name = name;
                Level = level;
                Gold = 1500;  // 기본값
                Hp = 100;     // 기본값
            }

            public void TakeDamage(int damage)
            {
                Console.WriteLine($"{Name}이(가) {damage}의 데미지를 받았습니다. 남은 체력: {Hp}");
            }

            //플레이어 정보 - 상태보기
            public void ShowStatus(List<Item> items)
            {
                int addAttack = 0;
                int addDefense = 0;

                Console.WriteLine("===== 지금장착한 아이템은 =====");
                foreach (Item item in items)
                {
                    if (item.IsEquipped == true)
                    {
                        Console.WriteLine($"{item.Name}| 공격력 : {item.AtackStat} 방어력 : {item.DefenseStat}");

                        addAttack += item.AtackStat;
                        addDefense += item.DefenseStat;
                    }

                }



                string input;
                do
                {
                    Console.WriteLine("===== 상태창 =====");
                    Console.WriteLine($"이름: {Name}");
                    Console.WriteLine($"레벨: {Level}");
                    Console.WriteLine($"직업: {Class}");
                    Console.WriteLine($"공격력: {Attack + addAttack}(+{addAttack})");
                    Console.WriteLine($"방어력: {Defense + addDefense}(+{addDefense})");
                    Console.WriteLine($"체력: {Hp}");
                    Console.WriteLine($"골드: {Gold} G");
                    Console.WriteLine("=================");
                    Console.WriteLine("0. 나가기");

                    input = Console.ReadLine();

                    if (input != "0")
                    {
                        Console.WriteLine("잘못된 입력입니다. 다시 시도하세요.");
                    }
                } while (input != "0");
            }


        }

        //직업
        public class Warrior : Player
        {
            public Warrior(string name, int level) : base(name, level)
            {
                Class = "Warrior";
                Attack = 10;
                Defense = 5;
            }
        }

        //아이템
        public class Item
        {
            public string Name { get; set; }
            public bool IsEquipped { get; set; }
            public int AtackStat { get; set; }
            public int DefenseStat { get; set; }
            public string Desc { get; set; }
            public int Price { get; set; }

            public Item(string name, int attackStat, int defenseStat, string desc, int price)
            {
                Name = name;
                IsEquipped = false;
                AtackStat = attackStat;
                DefenseStat = defenseStat;
                Desc = desc;
                Price = price;
            }
        }

        //인벤토리
        public class Inventory
        {
            public List<Item> items;

            public Inventory(List<Item> initialItems)
            {
                items = initialItems;  // 전달된 리스트로 초기화
            }

            public void AddItem(Item item)
            {
                items.Add(item);
                Console.WriteLine($"{item.Name}이(가) 인벤토리에 추가되었습니다.");
            }

            public void RemoveItem(Item item)
            {
                items.Remove(item);
                Console.WriteLine($"{item.Name}이(가) 인벤토리에서 제거되었습니다.");
            }

            public void ShowInventory()
            {
                string input;
                do
                {
                    Console.WriteLine("===== 인벤토리 =====");

                    foreach (var item in items)
                    {
                        string equippedStatus = item.IsEquipped ? "[E]" : "";
                        Console.WriteLine($"{item.Name} {equippedStatus}");
                    }

                    Console.WriteLine("====================");
                    Console.WriteLine("1. 장착관리");
                    Console.WriteLine("0. 나가기");

                    input = Console.ReadLine();

                    switch (input)
                    {
                        case "1":
                            EquipmentManagement();
                            break;
                        case "0":
                            break;
                        default:
                            // 1도 0도 아닌 경우 실행
                            Console.WriteLine("잘못된 입력입니다. 다시 시도하세요.");
                            break;
                    }
                } while (input != "1" && input != "0");
            }
            //인벤토리 - 장착관리
            public void EquipmentManagement()
            {
                bool exit = true;
                do
                {
                    int index = 1;  // 번호를 매기기 위한 변수

                    Console.WriteLine("장착할 아이템 번호를 입력하세요:");
                    foreach (var item in items)
                    {
                        string equippedStatus = item.IsEquipped ? "[E]" : "";
                        Console.WriteLine($"{equippedStatus} {index}. {item.Name}");
                        index++;  // 다음 아이템을 위한 번호 증가
                    }

                    Console.WriteLine("====================");
                    Console.WriteLine("0. 나가기");


                    int itemNumber = int.Parse(Console.ReadLine()) - 1;


                    if (itemNumber >= 0 && itemNumber < items.Count)
                    {
                        var selectedItem = items[itemNumber];
                        selectedItem.IsEquipped = !selectedItem.IsEquipped;  // 장착 상태 반전
                        Console.WriteLine($"{selectedItem.Name}이(가) {(selectedItem.IsEquipped ? "장착" : "해제")}되었습니다.");
                    }
                    else if (itemNumber == -1)
                    {
                        exit = false;
                    }
                } while (exit);
            }
        }

        //상점
        public class Shop
        {
            public List<Item> items;
            public List<Item> shopItems;

            public Shop(List<Item> initialItems, List<Item> defaultShopItems)
            {
                items = initialItems;  // 내아이템
                shopItems = defaultShopItems;    //상점아이템
            }
            //상점 - 아이템보기
            public void ShowShop(Player player, Inventory inventory)
            {
                bool exit = true;
                do
                {


                    Console.WriteLine($"[보유골드] \n{player.Gold}\n");

                    Console.WriteLine("[아이템목록]");
                    foreach (var item in shopItems)
                    {
                        bool isOwned = items.Any(playerItem => playerItem.Name == item.Name);
                        string ownershipStatus = isOwned ? "보유 중" : "";

                        string equippedStatus = item.IsEquipped ? "[E]" : "";
                        Console.WriteLine($"{item.Name} | 공격력 : {item.AtackStat} 방어력 : {item.DefenseStat} | {item.Desc} | {(isOwned ? "구매완료" : item.Price + "G")}");
                    }

                    Console.WriteLine("====================");
                    Console.WriteLine("1. 아이템 구매");
                    Console.WriteLine("0. 나가기");

                    string num = Console.ReadLine();


                    switch (num)
                    {
                        case "1":
                            player.Gold = Shoping(player.Gold, inventory);

                            break;
                        case "0":
                            return;
                        default:
                            // 1도 0도 아닌 경우 실행
                            Console.WriteLine("잘못된 입력입니다. 다시 시도하세요.");
                            break;
                    }

                } while (exit);
            }

            //상점 - 아이템 구매
            public int Shoping(int gold, Inventory inventory)
            {
                bool exit = true;
                do
                {
                    Console.WriteLine($"[보유골드] \n{gold}\n");
                    int index = 1;
                    Console.WriteLine("[아이템목록]");
                    foreach (var item in shopItems)
                    {
                        bool isOwned = items.Any(playerItem => playerItem.Name == item.Name);
                        string ownershipStatus = isOwned ? "보유 중" : "";


                        string equippedStatus = item.IsEquipped ? "[E]" : "";
                        Console.WriteLine($"{index}.{item.Name} | 공격력 : {item.AtackStat} 방어력 : {item.DefenseStat} | {item.Desc} | {(isOwned ? "구매완료" : item.Price + "G")}");
                        index++;
                    }

                    Console.WriteLine("====================");
                    Console.WriteLine("0. 나가기");

                    int itemNumber = int.Parse(Console.ReadLine()) - 1;


                    if (itemNumber >= 0 && itemNumber < shopItems.Count)
                    {
                        var selectedItem = shopItems[itemNumber];

                        bool isOwned = items.Any(playerItem => playerItem.Name == selectedItem.Name);
                        if (isOwned)
                        {
                            Console.WriteLine("이미 구매한 아이템입니다.");
                        }
                        else if (gold < shopItems[itemNumber].Price)
                        {
                            Console.WriteLine("Gold 가 부족합니다. ");
                        }
                        else if (gold >= shopItems[itemNumber].Price)
                        {
                            gold -= shopItems[itemNumber].Price;
                            inventory.AddItem(selectedItem);
                            Console.WriteLine("asdf");

                        }
                    }
                    else if (itemNumber == -1)
                    {
                        exit = false;
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다.");
                    }

                } while (exit);
                Console.WriteLine($"----------{gold}------");
                return gold;
            }
        }



        //게임 입장
        static void EnterGame(Player player, Inventory inventory, Shop shop)
        {

            while (true)
            {
                Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
                Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
                Console.WriteLine("1. 상태보기");
                Console.WriteLine("2. 인벤토리");
                Console.WriteLine("3. 상점");


                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        player.ShowStatus(inventory.items);
                        break;
                    case "2":
                        inventory.ShowInventory();
                        break;
                    case "3":
                        shop.ShowShop(player, inventory);
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            //기본직업
            Warrior warrior = new Warrior("Chad", 1);

            //기본 플레이어 보유 아이템
            List<Item> startingItems = new List<Item>
            {
                new Item("무쇠갑옷",0, 5 ,"무쇠로 만들어져 튼튼한 갑옷입니다.",1000),
                new Item("스파르타의 창",7,0,"스파르타의 전사들이 사용했다는 전설의 창입니다.", 1000),
                new Item("낡은 검",2,0,"쉽게 볼 수 있는 낡은 검 입니다.", 1000)
            };

            //상점 아이템
            List<Item> defaultShopItems = new List<Item>
            {
                new Item("수련자 갑옷",0, 5 ,"수련에 도움을 주는 갑옷입니다.  ", 1000),
                new Item("무쇠갑옷",0,9,"무쇠로 만들어져 튼튼한 갑옷입니다."   , 1000),
                new Item("스파르타의 갑옷",0,15,"무쇠로 만들어져 튼튼한 갑옷입니다.", 3500),
                new Item("낡은 검",2,0,"무쇠로 만들어져 튼튼한 갑옷입니다.", 600),
                new Item("청동도끼",5,0,"무쇠로 만들어져 튼튼한 갑옷입니다.", 1500),
                new Item("스파르타의 창",7,0,"무쇠로 만들어져 튼튼한 갑옷입니다.", 1000)

            };

            Inventory inventory = new Inventory(startingItems);
            Shop shop = new Shop(startingItems, defaultShopItems);
            EnterGame(warrior, inventory, shop);
            Console.WriteLine("더 이상 개발된게 업성요~!");
        }
    }
}
