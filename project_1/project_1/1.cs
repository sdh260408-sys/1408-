using System;

namespace project_1
{
    internal class _1
    {
        public class Object
        {
            public int HP = 100;
            private Random rand = new Random();
            public int Attack()
            {
                return rand.Next(5, 15);
            }
        }
        public class Player : Object 
        {
            private Random rand = new Random();
            public new int heal()
            {
                return rand.Next(5, 15);
            }
        }
        public class Enemy : Object { }
        static void Main(string[] args)
        {
            Player player = new Player();
            Enemy enemy = new Enemy();
            Console.WriteLine("공격 또는 회복 입력.\n");
            while (player.HP > 0 && enemy.HP > 0)
            {
                int playerAttack = player.Attack();
                string input = Console.ReadLine();
                if (input == "회복")
                {
                    Random healRand = new Random();
                    int critical = healRand.Next(0, 10);
                    if (critical >= 7)
                    {
                        int criticalHeal = player.heal() * 2;
                        player.HP += criticalHeal;
                        Console.WriteLine($"플레이어가 {criticalHeal} 만큼 크게 회복했습니다.\n플레이어의 체력: {player.HP}");
                    }
                    else
                    {
                        int healAmount = player.heal();
                        player.HP += healAmount;
                        Console.WriteLine($"플레이어가 {healAmount} 만큼 회복했습니다.\n플레이어의 체력: {player.HP}");
                    }
                }
                else if (input == "공격")
                {
                    enemy.HP -= playerAttack;
                    Console.WriteLine($"플레이어의 공격이 {playerAttack} 를 입혔습니다.\n적의 체력: {enemy.HP}");
                    if (enemy.HP <= 0)
                    {
                        Console.WriteLine("승리");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. '공격' 또는 '회복'을 입력하세요.");
                    continue;
                }
                int enemyAttack = enemy.Attack();
                player.HP -= enemyAttack;
                Console.WriteLine($"적의 공격이 {enemyAttack} 를 입혔습니다.\n플레이어의 체력: {player.HP}");
                if (player.HP <= 0)
                {
                    Console.WriteLine("패배");
                    break;
                }
            }
        }
    }
}
