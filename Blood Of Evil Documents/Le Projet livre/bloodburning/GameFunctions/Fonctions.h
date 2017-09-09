/*
** convert.h for convert in /home/haddad_b/
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Sun Dec  2 15:57:44 2012 benjamin haddad
** Last update Tue Jan 15 19:20:35 2013 benjamin haddad
*/

#ifndef		CONVERT_H
# define	CONVERT_H
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>

# define	CST_ROW_SIZE 2

typedef struct s_convert_base
{
  int           start_stlen;
  int           end_stlen;
  int           start_i;
  int           end_i;
  int           pow;
  int           i;
  char          *nb_end_base;
}		t_convert;

void            my_putchar(char c);
void            my_putstr(char *str);
/* void            putstr_ptr(char **tab) */
int             my_strlen(char *str);
int             my_putnbr(int nb);
int             my_getnbr(char *str);
/* int             *malloc_tab(int value); */
/* int             **malloc_ptr_tab(int dimension, int value); */
/* void            *display_tab(int *tab, int value); */
/* void            **display_ptr_tab(int **tab, int dimension, int value); */
/* char            **my_str_to_wordtab(char *str) */
/* char            **my_str_to_wouk(char *str, int istr, char **tab, int itab) */
/* int             power(int nb, int pow); */
/* int             *sort_int_min_tab(int *tab) */
/* int             *convert_base(int nb, char *start_base, char *end_base); */
/* t_convert       *convert_manage(t_convert *convert, char *start_base, char *end_base, int nb); */

#endif
