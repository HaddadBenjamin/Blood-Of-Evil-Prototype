/*
** fonctions.c for fonctions in /home/haddad_b//soutient
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Fri Nov 30 23:26:10 2012 benjamin haddad
** Last update Fri Jan  4 15:38:39 2013 benjamin haddad
*/

#include "Fonctions.h"

void		my_putchar(char c)
{
  write(1, &c, 1);
}

void		my_putstr(char *str)
{
  int		i;

  i = 0;
  while (str[i] != '\0')
    {
      my_putchar(str[i]);
      i++;
    }
}

int		my_strlen(char *str)
{
  int		i;

  i = 0;
  while (str[i] != '\0')
    i++;
  return (i);
}

int		my_putnbr(int nb)
{
  int		div;

  div = 1;
  if (nb < 0)
    {
      my_putchar('-');
      nb = nb * -1;
    }
  while (nb / div > 9)
    div = div * 10;
  while (div != 0)
    {
      my_putchar(nb / div % 10 + '0');
      div = div / 10;
    }
}

int             my_getnbr(char *str)
{
  int           i;
  int           sign;
  int           nb;

  i = 0;
  sign = 1;
  nb = 0;
  while (str[i] == '-' || str[i] == '+')
    {
      if (str[i] == '-')
        sign = sign * -1;
      i++;
    }
  if (str[i] < '0' || str[i] > '9' || str[i] == 0)
    {
      my_putstr("You can't transform this string to a number because one");
      my_putstr(" or several character of this string are not expect\n");
      return (NULL);
    }
  while (str[i] != '\0' && str[i] >= '1' && str[i] <= '9')
    {
      nb = nb * 10 + (str[i] - '0');
      i++;
    }
  return (sign * nb);
}
