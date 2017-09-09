/*
** Basicfunctions4.c for  in /home/haddad_b//Minishell
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Fri Mar  1 18:43:57 2013 benjamin haddad
** Last update Wed Mar 20 14:06:18 2013 benjamin haddad
*/

#include "fonctions.h"

int		my_strcmp_read(char *str1, char *str2)
{
  int		i;

  i = 0;
  while (str1[i] == str2[i] && str2[i] != 0 &&
	 my_strlen(str1) == my_strlen(str2))
    i++;
  if (str2[i] == 0)
    return (1);
  else
    return (-1);
}

void		my_putstr2(char *c)
{
  int		a;

  a = write(2, c, my_strlen(c));
  if (a == -1)
    {
      my_putstr("Error on write\n");
      exit(EXIT_FAILLURE);
    }
}

void		my_putchar2(char c)
{
  int		a;

  a = write(2, &c, 1);
    if (a == -1)
    {
      my_putstr("Error on write\n");
      exit(EXIT_FAILLURE);
    }
}

void		my_putnbr2(int nb)
{
  int		div;

  div = 1;
  if (nb < 0)
    {
      my_putchar2('-');
      nb = nb * -1;
    }
  while (nb / div > 9)
    div = div * 10;
  while (div != 0)
    {
      my_putchar2(nb / div % 10 + '0');
      div = div / 10;
    }
}

int		my_atoi_base(char *str, char *base)
{
  t_base	b;

  b.lenbase = my_strlen(base);
  b.i = 0;
  b.i1 = my_strlen(str) - 1;
  b.i2 = 0;
  if (str[0] == 0 || base[0] == 0)
    {
      my_putstr2("\nError on my atoi_base\n\n");
      exit(EXIT_FAILLURE);
    }
  b.nb = 0;
  while (b.i1 >= 0)
    {
      while (str[b.i1] != base[b.i2])
        b.i2++;
      if (b.i == 0)
        b.nb = b.nb + b.i2;
      else if (b.i != 0)
        b.nb = b.nb + b.i2 * power(b.lenbase, b.i);
      b.i2 = 0;
      b.i1--;
      b.i++;
    }
  return (b.nb);
}
