# Estado do Projeto — O Alquimista das Estações

## Objetivo atual
Estruturar a base inicial do projeto 2D Top-Down (URP) na Unity, validar a conexão com o servidor MCP e estabelecer as diretrizes de arquitetura e fluxo de trabalho em `AGENTS.md`.

## Feito
- Conexão com o Unity Editor validada via MCP (`console.write` com `"MCP conectado"`).
- Estrutura de pastas criada em `Assets/`:
  - `Assets/Scripts`
  - `Assets/Prefabs`
  - `Assets/Sprites`
  - `Assets/Audio`
  - `Assets/Animations`
  - `Assets/UI`
- Criado `AGENTS.md` na raiz do projeto com as regras arquiteturais, de UI, MCP e fluxo Git.
- Criado `docs/ESTADO.md` para acompanhamento contínuo entre sessões.

## Próximo passo
- Definir e implementar a estrutura base da cena inicial (`SampleScene` ou cena principal), incluindo o jogador Top-Down com `Rigidbody2D`, `Collider2D` e movimentação via novo Input System, além dos Managers iniciais (`GameManager`).

## Decisões tomadas
- **Engine & Pipeline:** Unity 2D com Universal Render Pipeline (URP).
- **Física & Controles:** Física 2D (`Rigidbody2D`, `Collider2D`, `OnTriggerEnter2D`) + novo Input System.
- **Padrões de Código:** Scripts centralizados em `Assets/Scripts/` aplicando Singleton (Managers), Observer (eventos de HUD/coleta) e Prefabs.
- **UI:** Padrão único para menus e HUD — Canvas com TextMeshPro (`Scale With Screen Size`, `1280x720`) ou UI Toolkit.
- **Workflow Unity/Git:** Manipulação de cenas/prefabs exclusivamente via ferramentas MCP (com leitura prévia da cena), salvamento ao fim de cada tarefa e atualização de `docs/ESTADO.md` antes do commit (pedindo confirmação antes de `git push`).
