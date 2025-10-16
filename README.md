🔥 FIREWALL Security App v0.1

Um aplicativo de segurança simples com uma interface moderna e monitoramento em tempo real de arquivos, desenvolvido em C# com a tecnologia WPF. Este projeto serve como um estudo sobre a criação de serviços do Windows e a sua comunicação com uma interface de usuário.

(Dica: Tire um screenshot do seu app, faça upload em um site como o Imgur e substitua o link acima)

✨ Funcionalidades

Interface Moderna: Design limpo e intuitivo construído em WPF.

Animações Visuais: Um anel de progresso animado que exibe o andamento de uma verificação simulada.

Serviço de Proteção em Tempo Real: Um Serviço do Windows que roda em segundo plano e monitora a pasta de Downloads por novos arquivos.

Lógica de Deteção Simples: O serviço verifica novos arquivos por nomes suspeitos (ex: "hack", "trojan") como uma prova de conceito.

Logs em Tempo Real: A interface exibe as atividades do serviço de proteção em tempo real, lendo um arquivo de log compartilhado.

🚀 Como Instalar e Executar (v0.1)

Este software é composto por duas partes: a Interface do Usuário (UI) e o Serviço de Proteção.

Requisitos

Windows 10 ou superior

.NET Framework 4.7.2 ou superior

Instalação do Serviço de Proteção

O serviço é o cérebro que monitora os arquivos e precisa ser instalado no sistema.

Compile a Solução: Abra o projeto no Visual Studio e compile em modo Release.

Abra o Prompt de Comando como Administrador.

Navegue até a pasta de compilação do serviço:

cd caminho\para\o\projeto\FirewallService\bin\Release


Crie a pasta de logs (se não existir):

mkdir C:\temp


Instale o serviço usando o sc.exe:

sc.exe create FirewallSecurityService binPath= "caminho\completo\para\FirewallService.exe"


(Importante: O binPath deve ser o caminho absoluto para o executável do serviço. Mantenha o espaço depois de binPath=)

Inicie o serviço:

sc.exe start FirewallSecurityService


Executando a Interface

Navegue até a pasta de compilação da interface:

cd caminho\para\o\projeto\FIREWALL\bin\Release


Execute o arquivo FIREWALL.exe. A interface irá iniciar e começar a exibir os logs do serviço que já está a rodar.
