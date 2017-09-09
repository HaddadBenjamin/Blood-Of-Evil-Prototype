/*
** fonctions.c for fonctions in /home/haddad_b//soutient
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Fri Nov 30 23:26:10 2012 benjamin haddad
** Last update Wed Mar 20 14:05:56 2013 benjamin haddad
*/

#include "fonctions.h"

void		my_putchar(char c)
{
  int		a;

  a = write(1, &c, 1);
  if (a == -1)
    {
      my_putstr("Error on write\n");
      exit(EXIT_FAILLURE);
    }
}

void		my_putstr(char *str)
{
  int		a;

  a = write(1, str, my_strlen(str));
  if (a == - 1)
    {
      my_putstr("Error on write\n");
      exit(EXIT_FAILLURE);
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

void		my_putnbr(int nb)
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

int		my_getnbr(char *str)
{
  int		i;
  long int	nb;
  int		sign;

  i = 0;
  nb = 0;
  sign = 1;
  while (str[i] == '+' || str[i] == '-')
    {
      if (str[i] == '-')
        sign = sign * -1;
      i++;
    }
  while (str[i] != '\0')
    {
      if (str[i] - 48 > '0' && str[i] - 48 < '9')
        print_only_number();
      else
        nb = nb * 10 + str[i] - 48;
      i++;
    }
  nb = error_getnbr(nb, sign);
  return (nb);
}
