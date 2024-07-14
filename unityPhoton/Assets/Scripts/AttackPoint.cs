using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Silly
{
    public class AttackPoint : MonoBehaviour
    {
        //public Enemy enemy;
        public Player player;
        public int damage = 1;
        public enum Char
        {
            Player,
            Enemy,

        }
        public Char character;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            switch (character)
            {
                case Char.Player:
                    if (other.CompareTag("Enemy"))
                    {
                        if (player.isAttackCheck)
                        {
                            //Debug.Log(other.tag);
                            //enemy = other.GetComponent<Enemy>();
                            player.isAttackCheck = false;
                            //enemy.SetHp(damage);
                        }
                    }
                    break;
                case Char.Enemy:
                    if (other.CompareTag("Player"))
                    {
                        //if (enemy.isAttackCheck)
                        {
                            //Debug.Log(other.tag);
                            player = other.GetComponent<Player>();
                            //enemy.isAttackCheck = false;
                            player.SetHp(damage);
                        }
                    }
                    break;
            }

        }
    }
}
