/*
** Fonctions.h for  in /home/haddad_b//FonctionsUtile/AllProject
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Wed Mar 20 11:27:49 2013 benjamin haddad
** Last update Fri Mar 22 17:16:53 2013 benjamin haddad
*/

#ifndef		FONCTIONS_H_
# define	FONCTIONS_H_
# define	CARACTER	256
# define	NUMBER_ANSWER	5
# define	EXIT_FAILLURE	-1
# define	EXIT_SUCCESS	0
# define	RED		"\033[0m\033[31m"
# define	GREEN		"\033[0m\033[32m"
# define	YELLOW		"\033[0m\033[32m"
# define	BLUE		"\033[0m\033[32m"
# define	WHITE		"\033[0m\033[37m"

#include <stdlib.h>
#include <stdio.h>
#include <curses.h>
#include <ncurses.h>
#include <time.h>
#include <termios.h>
#include <unistd.h>
#include <string.h>
#include <math.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <signal.h>

typedef struct	s_list
{
  char		*str;
  const int	listnumber;
  struct s_list	*next;
  struct s_list	*prev;
}		t_list;

typedef struct	s_get
{
  int		i;
  char		*str;
  int		div;
  int		mallocage;
}		t_get;

typedef struct	s_read
{
  char		buffer[CARACTER];
  char		*buff;
  ssize_t	size;
  int		answer[NUMBER_ANSWER - 1];
  int		fd;
}		t_read;

typedef struct	s_base
{
  int		i1;
  int		i2;
  int		nb;
  int		i;
  int		lenbase;
}		t_base;

typedef struct	s_pause2
{
  int		i;
  int		d;
  int		count;
  char		*str;
}		t_pause2;

void		my_putchar(char c);
void		my_putstr(char *str);
int		my_strlen(char *str);
void		my_putnbr(int nb);
int		my_getnbr(char *str);

void		my_ptr_putstr(char **str);
int		my_ptr_strlen(char **str);
char		*my_getstr(int nb);
char		*my_rev_getstrcpy(char *str);
int		power(int nb, int pow);

int		display_tab(int *tab, int n);
int		my_strcmp(char *str1, char *str2);
void		init_numbers(int *tab, int n);
char		**my_str_to_wouk(char *str, int istr, char **tab, int itab);
char		**my_str_to_wordtab(char *str);

int		my_strcmp_read(char *str1, char *str2);
void		my_putchar2(char c);
void		my_putstr2(char *str);
void		my_putnbr2(int nb);
int		my_atoi_base(char *str, char *base);

char		*my_str(char *str);
char		*concate(char *str1, char *str2);

void		mister_clear();
void		mister_clear_no_pause();

int		error_only_number(char **av);
void		error_malloc(char *str);				/* exit */
void		error_ptr_empty(char **av);				/* exit */
int		error_getnbr(long int result, int neg);			/* exit */
void		error_read(int rid);					/* exit */

void		error_open(int openthis);				/* exit */
void		error_write(int writethis);				/* exit */
void		error_kill(int killthis);				/* exit */
void		error_general(char *str, int error);			/* exit */
void		error_empty(char *str);					/* exit */

int		pause_system();
void		pause_system2();
void		pause_system2_1(char *str, int count);
void		pause_system2_2();

void		print_only_number();					/* exit */

int		main();

void		*x_malloc(int sizeofmalloc, int caseofmalloc);		/* exit */
ssize_t		x_read(int fd, char *buffer, int caracter, t_read *rid);/* exit */
ssize_t		x_write(int fd, char *buffer, int caracter, t_read *rid);/* exit */
int		x_open(char *path, int flags);				/* exit */
int		x_kill(pid_t pid, int sig);				/* exit */

#endif		/* !FONCTIONS_H_ */
