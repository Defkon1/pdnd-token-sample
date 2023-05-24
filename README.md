# PDND Token Sample

![.NET Version: net7.0](https://img.shields.io/badge/.NET%20Version-net7.0-blue)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/defkon1/pdnd-token-sample/blob/master/LICENSE)

Esempio di autenticazione OAuth2 per la PDND con generazione della client assertion.

Il codice esposto è scritto a solo scopo dimostrativo e accademico, e va sicuramente irrobustito in alcuni punti prima di ipotizzare un suo utilizzo in un ambiente di produzione.

Il codice viene rilasciato "così com'è" senza alcun tipo di garanzia implicita o esplicita come definito dalla Licenza MIT.


# Punti di interesse

La solution è composta da due progetti: la libreria principale PDNDTokenSample.Core, che contiene classi e modelli che implementano la logica base, e una console application che ne mostra l'utilizzo.

## Libreria principale

La libreria principale contiene la classe `PDNDTokenClient` che espone due metodi pubblici, uno per la generazione della client assertion e uno per effettuare l'autenticazione OAuth2 mediante client assertion JWS come da manuale tecnico. All'interno del codice del client è possibile vedere due modalità diverse di lettura del materiale crittografico, tramite lettura della coppia delle chiavi in formato PEM (mediante la classe `PemReader` del pacchetto NuGet [Portable.BouncyCastle](https://www.nuget.org/packages/Portable.BouncyCastle)) o direttamente leggendo la chiave privata.

La client assertion è firmata utilizzando il pacchetto NuGet [jose-jwt](https://www.nuget.org/packages/jose-jwt).

## Console application

La console application a corredo è utilizzata per mostrare l'utilizzo della libreria.

Può essere configurata tramite file `application.json` (con supporto per i multipli environment), variabili d'ambiente o riga di comando, e procede a creare una client assertion ed effettuare un tentativo di autenticazione utilizzando i parametri forniti.

L'applicazione utilizza il pacchetto NuGet [Spectre.Console](https://www.nuget.org/packages/Spectre.Console) per la formattazione dell'output in console.

![Esempio della console application](https://github.com/Defkon1/pdnd-token-sample/blob/main/docs/assets/console-application.png)


# License

[MIT](https://github.com/defkon1/pdnd-token-sample/blob/master/LICENSE) © [Alessio Marinelli](https://www.alessiomarinelli.it/)