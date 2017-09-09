/*
** basicfunctions5.c for  in /home/haddad_b//FonctionsUtile/AllProject
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Wed Mar 20 14:05:23 2013 benjamin haddad
** Last update Wed Mar 20 15:20:42 2013 benjamin haddad
*/

#include "fonctions.h"

char		*my_str(char *str)
{
  char		*finalstr;
  int		len;
  int		i;

  i = 0;
  len = my_strlen(str);
  finalstr = x_malloc(sizeof (*finalstr), len);
  while (i < len)
    {
      finalstr[i] = str[i];
      i++;
    }
  return (finalstr);
}

char		*concate(char *str1, char *str2)
{
  char		*str3;
  int		i;

  i = 0;
  str3 = x_malloc(sizeof (*str3), my_strlen(str1) + my_strlen(str2));
  while (str1)
    {
      str3[i] = str1[i];
      i++;
    }
  i = 0;
  while (str2)
    {
      str3[i] = str2[i];
      i++;
    }
  return (str3);
}
