/*
** Basicfunctions3.c for  in /home/haddad_b//FonctionsUtile/AllProject
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Mon Feb  4 11:26:22 2013 benjamin haddad
** Last update Wed Mar 20 14:06:10 2013 benjamin haddad
*/

#include "fonctions.h"

int		display_tab(int *tab, int n)
{
  int		a;

  a = 0;
  if (n <= 0)
    return (-1);
  while (a < n)
    {
      if (tab[a] == 0)
        tab[a] = -1;
      my_putnbr(tab[a]);
      my_putchar(' ');
      a++;
    }
  my_putchar('\n');
  return (1);
}

int		my_strcmp(char *str1, char *str2)
{
  int i;

  i = 0;
  while (str1[i] == str2[i] && str2[i] != 0)
    i++;
  if (str2[i] == 0)
    return (1);
  else
    return (-1);
}

void		init_numbers(int *tab, int n)
{
  int           i;

  i = 0;
  while (i < n)
    {
      tab[i] = 0;
      i++;
    }
}

char		**my_str_to_wouk(char *str, int istr, char **tab, int itab)
{
  int           dtab;

  dtab = 0;
  while (str[istr] != '\0')
    {
      if (str[istr] != ' ' && str[istr] != '\t' && str[istr] != ':' &&
          str[istr] != '\f' && str[istr] != '\b' && str[istr] != '\n')
        {
          tab[dtab] = malloc(sizeof (**tab) * my_strlen(str));
          if (tab[dtab] == NULL)
            return (NULL);
          while (str[istr] != ':' && str[istr] != '\t' && str[istr] != ':' &&
                 str[istr] != '\b' && str[istr] != '\n' && str[istr] != 0)
	    tab[dtab][itab++] = str[istr++];
          while (str[istr] == ' ' || str[istr] == '\t' || str[istr] == ':' ||
                 str[istr] == '\n' || str[istr] == '\b' || str[istr] == '\f')
            istr++;
        }
      dtab++;
      itab = 0;
    }
  return (tab);
}

char		**my_str_to_wordtab(char *str)
{
  char          **tab;
  int           itab;
  int           istr;

  itab = 0;
  istr = 0;
  if (str[istr] == '\0')
    return (NULL);
  tab = malloc(sizeof (*tab) * my_strlen(str));
  if (tab == NULL)
    return (0);
  while (str[istr] == ' ' || str[istr] == '\t' || str[istr] == ':' ||
         str[istr] == '\n' || str[istr] == '\b' || str[istr] == '\f')
    istr++;
  tab = my_str_to_wouk(str, istr, tab, itab);
  if (tab == NULL)
    return (0);
  return (tab);
}
