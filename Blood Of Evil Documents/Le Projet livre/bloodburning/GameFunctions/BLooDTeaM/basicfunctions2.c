/*
** Basicfunctions2.c for  in /home/haddad_b//alumthishit
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Fri Feb 15 18:04:31 2013 benjamin haddad
** Last update Wed Mar 20 14:06:03 2013 benjamin haddad
*/

#include "fonctions.h"

void		my_ptr_putstr(char **str)
{
  while (*str != 0)
    my_putstr(*str++);
}

int		my_ptr_strlen(char **tab)
{
  int		i;

  i = 0;
  while (tab[i++] != '\0')
    i = i + my_strlen(tab[i]);
  return (i);
}

char		*my_getstr(int nb)
{
  t_get		get;

  get.div = 1;
  get.i = 0;
  get.mallocage = 0;
  while (nb / get.div > 9)
    {
      get.div = get.div * 10;
      get.mallocage++;
    }
  if (nb < 0)
    get.mallocage++;
  get.str = malloc(sizeof(*get.str) * get.mallocage);
  if (nb < 0)
    {
      get.str[get.i++] = '-';
      nb = nb * -1;
    }
  while (nb >= 1)
    {
      get.str[get.i++] = nb / get.div % 10 + '0';
      nb = nb / 10;
    }
  get.str = my_rev_getstrcpy(get.str);
  return (get.str);
}

char		*my_rev_getstrcpy(char *str)
{
  int		i;
  int		endstring;
  char		c;

  i = 0;
  if (str[i] == '-')
    i++;
  endstring = my_strlen(str) - 1;
  while (endstring >= i)
    {
      c = str[endstring];
      str[endstring] = str[i];
      str[i] = c;
      endstring--;
      i++;
    }
  return (str);
}
int		power(int nb, int pow)
{
  int		a;

  a = nb;
  if (pow == 0)
    nb = 1;
  while (pow-- > 1)
    {
      nb = nb * a;
      if (nb > 2147483647 || nb < -2147483647)
        {
          my_putstr2("The size of the power your number is too");
          my_putstr2("high to be store in an int\n");
          exit(EXIT_FAILLURE);
        }
    }
  if (pow < 0)
    {
      my_putstr2("The type int can't manage the negative power\n");
      exit(EXIT_FAILLURE);
    }
  return (nb);
}
