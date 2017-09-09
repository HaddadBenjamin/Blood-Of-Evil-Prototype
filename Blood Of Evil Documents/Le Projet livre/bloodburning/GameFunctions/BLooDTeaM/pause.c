/*
** Pause.c for  in /home/haddad_b//FonctionsUtile/AllProject
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Tue Mar 19 11:33:40 2013 benjamin haddad
** Last update Thu Mar 21 11:16:51 2013 benjamin haddad
*/

#include "fonctions.h"

int		pause_system()
{
  int		i;

  i = 0;
  while (i < 250000000)
    i++;
  my_putchar('.');
  while (i < 500000000)
    i++;
  my_putchar('.');
  while (i < 750000000)
    i++;
  my_putchar('.');
  while (i < 830000000)
    i++;
  my_putchar('\r');
  return (i);
}

void		pause_system2()
{
  t_pause2		p;

  p.i = 0;
  p.d = 0;
  p.count = 0;
  p.str = x_malloc(sizeof(*p.str), 100);
  while (p.i <= 100)
    p.str[p.i++] = ' ';
  p.i = 0;
  while (p.i < 101)
    {
      pause_system2_1(p.str, p.count);
      while (p.d < 13000000)
        p.d++;
      p.d = 0;
      p.count++;
      p.str[p.i] = '|';
      p.i++;
    }
  my_putchar('\n');
  my_putchar('\n');
  if (p.i == 101)
    while (42)
      pause_system2_2();
}

void		pause_system2_1(char *str, int count)
{
  my_putstr("\033[0m\033[37m");
  my_putstr(" [ ");
  my_putstr("\033[0m\033[32m");
  my_putstr(str);
  my_putstr("\033[0m\033[37m");
  my_putstr("]\t");
  my_putstr("\033[0m\033[31m");
  my_putnbr(count);
  my_putstr("\033[0m\033[37m");
  my_putchar('%');
  my_putchar('\r');
}

void		pause_system2_2()
{
  int		d;

  d = 0;
  my_putstr("\033[0m\033[37m");
  my_putstr("\t\t\t\t\t   ");
  my_putstr("DOWNLOAD COMPLETE !\r");
  while (d < 90000000)
    d++;
  d = 0;
  my_putstr("                                       ");
  my_putstr("                       ");
  while (d < 90000000)
    d++;
  d = 0;
  my_putstr("\r");
}

void		pause_system3()
{
  char		*str;
  int		i;
  int		x;

  x = 0;
  str = x_malloc(sizeof(*str), 5);
  str = "|/-\\\0";
  while (42)
    {
      i = 0;
      while (i < 4)
	{
	  my_putstr("Dowloading... [");
	  my_putstr(RED);
	  my_putchar(str[i]);
	  my_putstr(WHITE);
	  my_putstr("]\r");
	  while (x < 40000000)
	    x++;
	  x = 0;
	  i++;
	}
    }
}
