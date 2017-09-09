/*
** Error2.c for  in /home/haddad_b//Rush
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Sat Mar  2 12:11:33 2013 benjamin haddad
** Last update Wed Mar 20 14:06:38 2013 benjamin haddad
*/

#include "fonctions.h"

void		error_open(int openthis)
{
  if (openthis == -1)
    {
      my_putstr2("\nError : Your open doesn't work and return -1 ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
}

void		error_write(int writethis)
{
  if (writethis == -1)
    {
      my_putstr2("\nError : Your write doesn't work and return -1 ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
}

void		error_kill(int killthis)
{
  if (killthis == -1)
    {
      my_putstr2("\nError : Your kill doesn't work and return -1 ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
}

void		error_empty(char *str)
{
  int           d;
  static int    numberofempty;

  d = 0;
  if (str[0] == 0 || str[0] == '0')
    {
      my_putstr2("\nError : Argument number ");
      my_putnbr2(d);
      my_putstr2(" of the ");
      my_putnbr2(numberofempty);
      my_putstr2("th test");
      my_putstr2(" is empty\n\n");
      exit(EXIT_FAILLURE);
    }
  numberofempty++;
}

void		error_general(char *str, int error)
{
  if (error == -1)
    {
      my_putstr2("Error on : ");
      my_putstr2(str);
      my_putchar('\n');
      exit(EXIT_FAILLURE);
    }
}
