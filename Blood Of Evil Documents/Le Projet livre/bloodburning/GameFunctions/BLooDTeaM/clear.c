/*
** Clear.c for  in /home/haddad_b//FonctionsUtile/AllProject
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Sun Feb 17 12:59:59 2013 benjamin haddad
** Last update Wed Mar 20 14:06:25 2013 benjamin haddad
*/

#include "fonctions.h"

void		mister_clear()
{
  pause_system();
  my_putstr("\033[2J");
  my_putstr("\033[0;0H");
}

void		mister_clear_no_pause()
{
  my_putstr("\033[2J");
  my_putstr("\033[0;0H");
}
