##
## Makefile for test in /home/haddad_b//afs/rendu/piscine/Jour_11
## 
## Made by benjamin haddad
## Login   <haddad_b@epitech.net>
## 
## Started on  Mon Oct 22 11:07:10 2012 benjamin haddad
## Last update Tue Jan 15 19:19:28 2013 benjamin haddad
##

CC = gcc

RM = rm -f

CFLAGS += -W -lm -Wextra -Wall -Werror -Wchar
CFLAGS += -ansi -pedantic
CFLAGS += -I.

NAME =	wolf3d

SRCS =	Basicfunctions.c Random_Item.c

OBJS=	$(SRCS:.c = .o)

all :	$(NAME)

$(NAME):$(OBJS)
	$(CC)  $(OBJS) -L/usr/lib64 -lmlx_$(HOSTTYPE) -lXext -lX11 -o $(NAME) $(LDFALGS)

clean:	$(OBJ)
	$(RM) $(OBJ)

fclean:	clean
	$(RM) $(NAME)

re:	fclean all
