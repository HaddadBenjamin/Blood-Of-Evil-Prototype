/*
** Print.c for  in /home/haddad_b//FonctionsUtile/AllProject
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Sun Feb 17 12:55:38 2013 benjamin haddad
** Last update Wed Mar 20 14:07:30 2013 benjamin haddad
*/

#include "fonctions.h"

void		print_only_number()
{
  my_putstr("\nError : Your string can't be transform to a number");
  my_putstr(" because it contains a alphabetical caracter");
  my_putstr("\nThe only available caracter ");
  my_putstr("are digit\n\n");
  exit(-1);
}
