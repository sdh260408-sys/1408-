using System;

namespace project_1
{
    internal class _1
    {
        public class Character
        {
            public int HP = 100;
            protected Random rand = new Random();

            public virtual int Attack()
            {
                return rand.Next(5, 15);
            }

            public void TakeDamage(int amount)
            {
                HP -= amount;
            }
        }
        public class Player : Character
        {
            public void Heal()
            {
                int critical = rand.Next(0, 10);
                int healAmount = rand.Next(5, 15);

                if (critical >= 4)
                {
                    int criticalHeal = healAmount * 2;
                    HP += criticalHeal;
                    Console.WriteLine($"플레이어가 {criticalHeal} 만큼 크게 회복했습니다.\n플레이어의 체력: {HP}");
                }
                else
                {
                    HP += healAmount;
                    Console.WriteLine($"플레이어가 {healAmount} 만큼 회복했습니다.\n플레이어의 체력: {HP}");
                }
            }
        }
        public class Enemy : Character { }
        static void Main(string[] args)
        {
            Player player = new Player();
            Enemy enemy = new Enemy();
            Console.WriteLine("공격 또는 회복 입력.\n");
            while (player.HP > 0 && enemy.HP > 0)
            {
                string input = Console.ReadLine();
                if (input == "회복")
                {
                    player.Heal();
                }
                else if (input == "공격")
                {
                    int playerAttack = player.Attack();
                    enemy.TakeDamage(playerAttack);
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
                player.TakeDamage(enemyAttack);
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
