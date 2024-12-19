# Prop Hunt Game
Creación de prototipo de videojuego prop hunt online en la version  2022.3.33f1  en unity

Cada partida se compondrá por 2 equipos y un timer el cual ira
descendiendo hasta 0.
Uno de los equipos es el que se esconde “Hiders” y el otro es el que ha de buscar y
eliminar a las que se esconden “Hunters”, los Hiders tiene la habilidad de transformarse
en objetos del entorno para camuflarse y evitar ser descubiertos, pero cada cierto tiempo
emiten un sonido para dar pistas a los Hunters, por otro lado los Hunters tienen armas
que sirven para eliminar a los Hiders a distancia pero estos empiezan a jugar un cierto
tiempo después de los Hiders además de que si no logran atrapar a todos los Hiders
antes de que se acabe el tiempo estos perderán.

Enlace a Github: https://github.com/Luxary-Arxer/Prop-Hunt-Game-Online

Enlace a Github Release: https://github.com/Luxary-Arxer/Prop-Hunt-Game-Online/releases/tag/Ejercico_4


- Build (.exe)
- Vídeo con demo explicando lo que se está entregando
- Paquete Unity

# Instrucciones
 
Escena principal: Home

Ejecutar el programa dos veces en el mismo ordenador o en diferentes ordenadores concertados en la misma red.
Abre dos instancias del juego, en la que quieras que sea el Server clica su botón correspondiente, el la del cliente introduce la Dirección IPv4 donde pone "Id Server" i presión el botón de join.

# Controles

###General

WASD -> Movimiento

Space -> Saltar

Shift -> Run

### Hunter

Left mouse -> Shot

###General

F -> Transformar en objeto que estas mirando

R -> Anular triangulaciones

# Contribuciones

### Nixon Correa:

• Uso de información del cliente y servidor para mover objetos.

• Añadido en una cola y optimización en el envío de paquetes.

• Enviar datos del servidor y recibir-los al cliente.

• Conectar mas de un cliente al servidor

### Enric Arxer:

• Concepción básica del servidor.

• Uso de información del cliente para rotar objeto.

• Generar jugador en el servidor cuando se realiza la conexión.

• Caviar el jugador entre Hunter y Alien

### Guillem Aixut:

• Enviar datos del cliente y recibir-los al servidor.

• Transformar los players en props

# Dificultades, comentarios o errores

- Error 1

# Cuando sea relevante, lista de mejoras de entregas anteriores

- Arreglado el error que no te permitía conectar-se a otro ordenador en que este en una red local
