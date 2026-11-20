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

## 3. Fluxo de Trabalho de Sessão e Git
- **Início de Sessão:** Sempre ler `docs/ESTADO.md` antes de iniciar qualquer tarefa para entender o contexto atual.
- **Fim de Tarefa:** Ao concluir cada tarefa:
  1. Salvar a cena ativa na Unity via MCP.
  2. Atualizar `docs/ESTADO.md` (mantendo-o enxuto, até 60 linhas).
  3. Preparar o `git commit` com as alterações realizadas (**sempre perguntar ao usuário antes de executar `git push`**).
