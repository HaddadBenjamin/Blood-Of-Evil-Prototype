/*
** Does_my_item_fall.c for  in /home/haddad_b//LangageC/BLooDBuRNiNG/GameFunctions/BLooDTeaM/BLooDBuRNiNG
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Sat Mar 23 16:28:18 2013 benjamin haddad
** Last update Sat Mar 23 18:34:53 2013 benjamin haddad
*/

#include "fonctions.h"

int		does_my_item_fall(int magicFind)
{
  int		numberOfItems;

  numberOfItems = magicFind % 100;
  magicFind /= 100;
  srand(time(NULL) * getpid() + 65);
  if (numberOfItems > rand() % 101)
    numberOfItems = magicFind + 1;
  else
    numberOfItems = magicFind;
  if (numberOfItems == 0)
    my_putstr("l'objet n'est pas tombé\n");
  else
    {
      my_putstr("l'objet est tombé ");
      my_putnbr(numberOfItems);
      my_putstr(" fois\n");
    }
  return (numberOfItems);
}

