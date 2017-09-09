/*
** Xfunctions.c for  in /home/haddad_b/
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Tue Mar 12 15:06:47 2013 benjamin haddad
** Last update Wed Mar 20 14:07:47 2013 benjamin haddad
*/

#include "fonctions.h"

void		*x_malloc(int sizeofmalloc, int caseofmalloc)
{
  static int	numberofread;
  void		*str;

  str = malloc(sizeof(sizeofmalloc) * (caseofmalloc + 1));
  if (str == NULL)
    {
      my_putstr2("\nError : Malloc number ");
      my_putnbr2(numberofread + 1);
      my_putstr2(" have failled ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
  return (str);
}

ssize_t		x_read(int fd, char *buffer, int caracter, t_read *rid)
{
  static int	numberofread;

  rid->fd = fd;
  rid->buff = buffer;
  rid->size = read(fd, buffer, caracter - 1) - 1;
  if (rid->size == 0)
    {
      my_putstr2("\nError : Read number ");
      my_putnbr2(numberofread + 1);
      my_putstr2(" is empty ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
  else if (rid->size == -1)
    {
      my_putstr2("\nError : Read number ");
      my_putnbr2(numberofread + 1);
      my_putstr2(" have failled ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
  rid->buff[rid->size] = '\0';
  numberofread++;
  return (rid->size);
}

ssize_t		x_write(int fd, char *buffer, int caracter, t_read *rid)
{
  static int	numberofwrite;

  rid->fd = fd;
  rid->buff = buffer;
  if (buffer[0] == 0)
    {
      my_putstr2("\nError : Write number ");
      my_putnbr2(numberofwrite + 1);
      my_putstr2(" is empty ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
  rid->size = write(fd, buffer, caracter - 1) - 1;
  if (rid->size == -1)
    {
      my_putstr2("\nError : Write number ");
      my_putnbr2(numberofwrite + 1);
      my_putstr2(" have failled ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
  numberofwrite++;
  return (rid->size);
}

int            x_open(char *path, int flags)
{
  static int	numberofopen;
  int		fd;

  fd = open(path, flags);
  if (fd == -1)
    {
      my_putstr2("\nError : Open number ");
      my_putnbr2(numberofopen + 1);
      my_putstr2(" have failled\n\n");
      exit(EXIT_FAILLURE);
    }
  return (fd);
}

int		x_kill(pid_t pid, int sig)
{
  int		killer;
  static int	numberofkill;

  /* killer = kill(pid, sig); */
  killer = pid * sig;
  if (killer == 0)
    {
      my_putstr2("\nError : Kill number ");
      my_putnbr2(numberofkill + 1);
      my_putstr2(" didn't send any signam ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
  else if (killer == -1)
    {
      my_putstr2("\nError : Kill number ");
      my_putnbr2(numberofkill + 1);
      my_putstr2(" have failled ");
      my_putstr2("This is why the program have stopped of himself\n\n");
      exit(EXIT_FAILLURE);
    }
  numberofkill++;
  return (killer);
}
