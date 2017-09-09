/*
** main.c for  in /home/haddad_b//FonctionsUtile/AllProject
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Tue Mar 19 12:55:51 2013 benjamin haddad
** Last update Sat Mar 30 11:14:25 2013 benjamin haddad
*/

#include "fonctions.h"

float		random_determinate_with_percent(float randompercent)	/* (fonction qui prend un nombre en pourcentage et renvoi 1 si un random de pourcentage est réussi sinon elle renvoi 0 */
{
  randompercent *= 1000;
  /* 33.333 */
/* 33333 */
  if (randompercent > random(100000))
    return (1);
  else
    return (0);
}

float		random_between_two_float(float min, float max)		/* fonction qui renvoi un nombre compris entre deux nombre décimaux */
{
  float		result;

  min = min * 1000;		/* le x 1000 permet d'éviter les nombrse a virgule ex : 33.333 = 33333 */
  max = max * 1000;
  result = random_between_a_and_b(min, max);
  result = result / 1000;
  return (result);
}

long int	critical_strike(int damage, int criticalpercent)
{
  int		howmanycritical;
  int		newdamage;

  newdamage = damage;
  howmanycritical = 0;
  if (criticalpercent > 50)
    criticalpercent = 50;
  if (rand_percent(criticalpercent) == 1)
    howmanycritical++;
  if (howmanycritical == 1)
    {
      if (rand_percent(criticalpercent) == 1)
	howmanycritical++;
    }
  if (howmanycritical > 0)
    newdamage = newdamage * un nombre entre 1.5 et 2.35;
  if (howmanycritcal > 1)
    newdamage = newdamage * un nombre entre 1.5 et 2.35;
  if (howmanycritica > 0)
    {
      my_putstr("My new damage are ");
      my_putnbr(newdamage);
      my_putstr("that is to say ");
      my_putnbr_float(newdamage / damage);
      my_putstr("higher than before\n");
      my_putstr(RED);
      if (newdamage / 5.0 > damage)
	my_putstr("Fatality\n");
      else if (newdamage / 4.0 > damage)
	my_putstr("Menstruality\n");
      else if (newdamage / 3.5 > damage)
	my_putstr("Attrocity\n");
      else if (newdamage / 3.0 > damage)
	my_putstr("Ferocity\n");
      else if (newdamage / 2.5 > damage)
	my_putstr("Bestiality\n");
      else if (newdamage / 2.0 > damage)
	my_putstr("Brutality\n");
      my_putstr(WHITE);
    }
  return (newdamage);
  1) determiner si on un coup critique
  2) si oui redeterminer si on a un 2 eme coup critique
  3) si on n'a pas du tout de copu critique renvoyer domage non modifier
4) si on 1 un coup critique damage = damage * [1.5 | 2.35]
si on a 2 coup critqiue damage = damage * [1.5 | 2.35] * [1.5 | 2.35]
puis renvoyer dommage
}	

int		main()
{
  int		damage;

 damage = critical_strike(5600, 33,33)
  /* my_putnbr(how_many_life_does_i_have(10000, 10100)); */
  /* my_putstr(" pour 138 "); */
  /* does_my_item_fall(138); */
  /* my_putstr(" pour 95 "); */
  /* does_my_item_fall(95); */
  /* my_putstr(" pour 68 "); */
  /* does_my_item_fall(68); */
  /* my_putstr(" pour 132 "); */
  /* does_my_item_fall(132); */
  /* my_putstr(" pour 148 "); */
  /* does_my_item_fall(148); */

  return (0);
}
