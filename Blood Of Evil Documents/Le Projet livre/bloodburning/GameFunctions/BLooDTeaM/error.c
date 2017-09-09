/*
** Error.c for  in /home/haddad_b//FonctionsUtile/AllProject
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Sun Feb 17 12:50:32 2013 benjamin haddad
** Last update Wed Mar 20 14:06:33 2013 benjamin haddad
*/

#include "fonctions.h"

int		error_only_number(char **av)
{
  int		d;
  int		v;
  int		i;

  d = 1;
  v = 0;
  while (av[d] != 0)
    {
      while (av[d][v] != 0)
        {
          i = av[d][v] - 48;
          if (i < 0 || i > 9)
            {
              my_putstr2("\nError : Your argument contains alphabetic ");
              my_putstr2("caracter\nThe only available caracter ");
              my_putstr2("are number\n\n");
              return (-1);
            }
          v++;
        }
      v = 0;
      d++;
    }
  return (1);
}

void		error_malloc(char *str)
{
  if (str == NULL)
    {
      my_putstr2("\nError : Your malloc didn't work and return 0 ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
}

void		error_ptr_empty(char **av)
{
  int		d;
  static int	test;

  d = 1;
  while (av[d] != NULL)
    {
      if (av[d][0] == 0 || av[d][0] == '0')
        {
          my_putstr2("\nError : Argument number ");
	  my_putnbr2(d);
	  my_putstr2(" of the ");
	  my_putnbr2(test);
	  my_putstr2("th test");
	  my_putstr2(" is empty\n\n");
          exit(EXIT_FAILLURE);
        }
      d++;
    }
  test++;
}

int		error_getnbr(long int result, int neg)
{
  result = result * neg;
  if (result >= 2147483647)
    {
      my_putstr2("\nError : This string can't be transform in a number\n");
      my_putstr2("This number is too high to be store in an int\n");
      my_putstr2("For this reason, the program will stopped by himself\n\n");
      exit(EXIT_FAILLURE);
    }
  if (result <= -2147483647)
    {
      my_putstr2("\nError : This string can't be transform in a number\n");
      my_putstr2("This number is too low to be store in an int\n");
      my_putstr2("For this reason, the program will stopped by himself\n\n");
      exit(EXIT_FAILLURE);
    }
  return (result);
}

void		error_read(int rid)
{
  if (rid <= 0)
    {
      my_putstr2("\nError : Your read  didn't work and return 0 ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
}
