/*
** how_many_life_does_i_have.c for how_many_life_does_i_have in /home/avice_d//Projets
** 
** Made by damien avice
** Login   <avice_d@epitech.net>
** 
** Started on  Thu Mar 28 15:07:23 2013 damien avice
** Last update Thu Mar 28 15:13:18 2013 damien avice
*/

int	how_many_life_does_i_have(int vie_actu, int vie_perdu)
{
  int		result;

  if (vie_actu <= vie_perdu)
    {
      my_putstr("Vous etes mort dans d'atroce souffrance\n");
      return (0);
    }
  else
    {
      result = vie_actu - vie_perdu;
      my_putstr("Vous etes toujours en vie\n");
      vie_actu = result;
      return (vie_actu);
    }
}
