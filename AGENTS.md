# Diretrizes do Agente — O Alquimista das Estações

Este arquivo define as regras arquiteturais e de fluxo de trabalho que qualquer agente de IA deve seguir ao trabalhar neste projeto.

## 1. Arquitetura e Tecnologias do Jogo
- **Gênero e Renderização:** O jogo é **2D Top-Down (URP)**.
- **Física e Input:** Utiliza **Física 2D** (`Rigidbody2D`, `Collider2D`, `OnTriggerEnter2D`) e o **Input System novo** (`UnityEngine.InputSystem`).
- **Organização de Código:** Todos os scripts C# devem ficar em `Assets/Scripts/` e seguir os padrões de projeto:
  - **Singleton:** Para Managers globais (ex.: `GameManager`, `SeasonManager`, `AudioManager`).
  - **Observer:** Uso de eventos C# (`event Action`, `UnityEvent`) para desacoplar HUD, coleta de itens e mudanças de estado.
  - **Prefabs:** Elementos reutilizáveis do jogo devem ser estruturados e instanciados como Prefabs em `Assets/Prefabs/`.
- **Interface de Usuário (UI):** Menus e HUD devem usar um **padrão único** em todo o projeto:
  - **Canvas (uGUI + TextMeshPro)** configurado com `UI Scale Mode: Scale With Screen Size` (resolução de referência `1280x720`), **ou** **UI Toolkit**, sem misturar abordagens.

## 2. Regras de Manipulação da Unity (MCP)
- **Uso Exclusivo do MCP para Cenas e Prefabs:** Qualquer alteração na cena ou em prefabs deve ser feita **apenas pelas ferramentas MCP do Unity** (`scene`, `gameobject`, `component`, `prefab`, etc.). **Nunca** edite arquivos `.unity` ou `.prefab` diretamente como texto/YAML.
- **Sincronização com a Equipe:** Sempre leia o estado atual da cena via MCP **antes** de realizar qualquer alteração, pois a equipe também edita o projeto manualmente no Unity Editor.

## 3. Fluxo de Trabalho de Sessão, Git e GitHub
- **Início de Sessão:** Sempre ler `docs/ESTADO.md` antes de iniciar qualquer tarefa para entender o contexto atual.
- **Branches e Issues (Nunca na `main`):**
  - **Nunca** desenvolva novas etapas diretamente na branch `main`.
  - **Antes de iniciar qualquer tarefa:** verifique se a working tree está limpa (`git status`), sincronize a `main` (`git checkout main && git pull`) e crie uma branch específica para a etapa usando o padrão `feat/<nome-da-etapa>` (ou `fix/<nome>`, `art/<nome>`).
  - Se existir uma Issue correspondente no GitHub (verificar via `gh issue list`), vincule a branch à issue com `gh issue develop <numero> --checkout`.
  - **Durante a etapa:** trabalhe apenas nessa branch.
- **Fim de Tarefa e Pull Request:** Ao concluir cada etapa:
  1. Salvar a cena ativa na Unity via MCP.
  2. Atualizar `docs/ESTADO.md` (mantendo-o enxuto, até 60 linhas).
  3. Fazer o `git commit` com mensagem descritiva.
  4. **Sempre perguntar ao usuário** antes de rodar `git push -u origin <branch>` e abrir o Pull Request via `gh pr create --base main` (com título claro e resumo do que foi feito e testado).
