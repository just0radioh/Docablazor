namespace DocaBlazor.Data;

/// <summary>
/// Repositório em memória com a árvore de conteúdo da base de conhecimento.
/// Equivalente à constante NAV_TREE do protótipo original em HTML/JS.
/// Registrado como singleton — os dados são estáticos e compartilhados
/// por todas as sessões do Blazor Server.
/// </summary>
public class DocsRepository
{
    public IReadOnlyList<NavNode> Tree { get; }

    private readonly Dictionary<string, NavNode> _byId = new();

    public DocsRepository()
    {
        Tree = BuildTree();
        IndexAndLinkParents(Tree, null);
    }

    public NavNode? FindById(string id) => _byId.GetValueOrDefault(id);

    private void IndexAndLinkParents(IEnumerable<NavNode> nodes, NavNode? parent)
    {
        foreach (var node in nodes)
        {
            node.Parent = parent;
            _byId[node.Id] = node;
            if (node.Children is not null)
                IndexAndLinkParents(node.Children, node);
        }
    }

    private static List<NavNode> BuildTree() => new()
    {
        new NavNode
        {
            Id = "topdesk", Label = "Top Desk", Icon = "🎧",
            Children = new()
            {
                new NavNode
                {
                    Id = "acompanhamento", Label = "Acompanhamento",
                    Children = new()
                    {
                        new NavNode
                        {
                            Id = "status-chamado", Label = "Status do Chamado",
                            Content = "Ao receber um chamado, é fundamental compreender integralmente a solicitação antes de direcioná-la à equipe. Todo o processo deve ser acompanhado em conjunto com o solicitante, caso necessário.\n\nChamados complexos ou que demandem ações de outros times são devidamente documentados, repassados via JIRA e reportados ao solicitante.",
                            Db = "sem banco interno"
                        }
                    }
                },
                new NavNode
                {
                    Id = "encerramento", Label = "Encerramento",
                    Content = "Para efetuar o encerramento de um chamado, todas as informações devem ser alinhadas previamente com o solicitante via Teams ou pelo próprio ticket.\n\nAlém disso, é obrigatório anexar evidências de resolução do problema ou bug (ex: capturas de tela ou confirmação formal de validação do solicitante).",
                    Db = "sem banco interno"
                },
                new NavNode
                {
                    Id = "aplicacoes-atendidas", Label = "Aplicações Atendidas",
                    Content = "O Top Desk centraliza o suporte a solicitações, reportes de bugs, melhorias e novas funcionalidades para as seguintes aplicações:\n\n• Cacau Digital\n• Cacau Lovers\n• OMS\n• QR Code Generator",
                    Db = "sem banco interno"
                },
                new NavNode
                {
                    Id = "endpoints-topdesk", Label = "Endpoints",
                    Content = "Atualmente há uma integração do TOPdesk com o Cacau Digital OMS e Portal do Revendedor. Com o Cacau Digital OMS, o TOPdesk lê a API para trazer o cadastro das lojas e seus representantes, para vincularmos aos chamados.",
                    Db = "sem banco interno"
                }
            }
        },
        new NavNode
        {
            Id = "cacau-digital", Label = "Cacau Digital", Icon = "📱",
            Children = new()
            {
                new NavNode
                {
                    Id = "site-pedidos", Label = "Site de Pedidos",
                    Children = new()
                    {
                        new NavNode
                        {
                            Id = "site-pedidos-oque", Label = "O que é",
                            Content = "Plataforma que permite aos franqueados realizarem pedidos de produtos Cacau Show de maneira ágil, centralizada e eficiente.\n\n(As APIs do site são gerenciadas pelo time de Desenvolvimento e estão hospedadas nos repositórios do GitHub e Azure).\n\nBancos envolvidos: MDM e OMS.",
                            Db = "MDM / OMS"
                        }
                    }
                },
                new NavNode
                {
                    Id = "relatorios", Label = "Relatórios",
                    Content = "Os relatórios analíticos do Cacau Digital são mantidos pela equipe de Data & Insights, responsáveis pela análise de vendas e performance corporativa via Power BI.\n\nNota: A API de integração entre o Cacau Digital e o Power BI não é gerenciada pela equipe Dev.",
                    Db = "Power BI / Databricks"
                },
                new NavNode
                {
                    Id = "extratos", Label = "Extratos",
                    Content = "Módulo responsável pela consulta de extratos no portal Cacau Digital, mantido pela equipe de Desenvolvimento.\n\nUtiliza a string de conexão ConnectionString_Portal (banco: cacaudigital_web_prd), compartilhada também com o Napp.",
                    Db = "ConnectionString_Portal (cacaudigital_web_prd)"
                },
                new NavNode
                {
                    Id = "historico-saf", Label = "Histórico do SAF",
                    Content = "Módulo destinado à consulta do histórico de chamados do SAF via banco Sysaid.\n\nNota: Os chamados via SAF foram oficialmente descontinuados em outubro de 2025.",
                    Db = "Sysaid"
                },
                new NavNode
                {
                    Id = "gerenciador-acessos", Label = "Gerenciador de Acessos",
                    Content = "Ferramenta para gestão de vínculos entre usuários e unidades de negócios.\n\nAo pesquisar pelo código SAP da loja, são exibidos todos os logins associados. Da mesma forma, ao buscar pelo e-mail do usuário, retornam-se todas as lojas vinculadas ao respectivo cadastro.",
                    Db = "ConnectionString_Portal (cacaudigital_web_prd)"
                },
                new NavNode
                {
                    Id = "acesso-portal", Label = "Acesso ao Portal",
                    Content = "O controle de permissões e a liberação de acessos ao portal Cacau Digital são realizados diretamente pela equipe de Suporte.",
                    Db = "Não Possui"
                },
                new NavNode
                {
                    Id = "aceite-contrato", Label = "Aceite de Contrato",
                    Content = "Aba destinada à consulta e assinatura digital de termos e contratos vigentes por parte dos franqueados, visando otimizar a organização legal.",
                    Db = "ConnectionString_Portal (cacaudigital_web_prd)"
                },
                new NavNode
                {
                    Id = "cadastro-loja", Label = "Cadastro de Loja",
                    Content = "Centraliza as informações operacionais da unidade: dados do gerente, contatos de WhatsApp, códigos SAP e RCKY, horários de funcionamento, inventário de equipamentos, status operacional, localização, limites de compra e redes sociais.\n\nOs dados mestres são integrados via SAP e refletidos no MDM; a manutenção das demais informações é de responsabilidade do franqueado.",
                    Db = "MDM (mdm_cacaushow_prd)"
                },
                new NavNode
                {
                    Id = "inventario-cd", Label = "Inventário",
                    Content = "Funcionalidade responsável pelo controle de estoque e gerenciamento dos produtos disponíveis no portal Cacau Digital.",
                    Db = "Inventario (inventario)"
                }
            }
        },
        new NavNode
        {
            Id = "cacau-lovers", Label = "Cacau Lovers", Icon = "❤️",
            Children = new()
            {
                new NavNode
                {
                    Id = "programa-fidelidade", Label = "Programa de Fidelidade",
                    Content = "Plataforma de fidelização que gerencia a distribuição de pontos, concedendo experiências exclusivas e descontos aos clientes.\n\nO ecossistema possui bases de dados estruturadas em ambiente de Produção e Homologação (HML).",
                    Db = "MongoDB"
                },
                new NavNode
                {
                    Id = "pontuacao", Label = "Cacaus (Pontuação)",
                    Children = new()
                    {
                        new NavNode
                        {
                            Id = "como-pontuar", Label = "Como pontuar",
                            Content = "Regra de acúmulo: A cada R$ 20,00 em compras efetuadas, o cliente recebe 1 'CACAU' (ponto).\n\nOs pontos acumulados podem ser trocados por produtos, descontos especiais e benefícios parceiros.",
                            Db = "MongoDB"
                        },
                        new NavNode
                        {
                            Id = "validade-pontos", Label = "Validade dos pontos",
                            Content = "Os pontos gerados têm validade de 30 dias contados a partir da data da compra. A atualização do extrato no perfil do usuário pode levar até 24 horas.",
                            Db = "MongoDB"
                        }
                    }
                },
                new NavNode
                {
                    Id = "resgates", Label = "Resgates",
                    Content = "Seção de trocas de pontos por benefícios (promoções, isenção de frete, descontos na próxima compra, itens da cafeteria em Mega Stores e doações).\n\nRegras Gerais:\n• Todas as recompensas podem sofrer alterações sem aviso prévio.\n• Limite de resgate: até 2 utilizações por CPF.\n• A gestão do catálogo é feita pelo painel Admin do Cacau Lovers.",
                    Db = "MongoDB"
                },
                new NavNode
                {
                    Id = "front-lovers", Label = "Front",
                    Content = "A interface do usuário (Frontend) é desenvolvida, mantida e testada de forma contínua pelo time de DEV.",
                    Db = "Fidelidade banco"
                },
                new NavNode
                {
                    Id = "dados-lovers", Label = "Dados",
                    Content = "O Cacau Lovers conta com uma infraestrutura dedicada de Banco de Dados para sustentação da aplicação em Produção e Homologação.",
                    Db = "MongoDB"
                }
            }
        },
        new NavNode
        {
            Id = "oms", Label = "OMS", Icon = "📦",
            Children = new()
            {
                new NavNode
                {
                    Id = "integracoes-oms", Label = "Integrações",
                    Content = "Módulo focado na gestão de integrações, microsserviços e rotinas de comunicação ligadas ao ecossistema do OMS.",
                    Db = "oms_cacaushow_prd"
                },
                new NavNode
                {
                    Id = "banco-oms", Label = "OMS (oms_cacaushow_prd)",
                    Content = "Conexão: OMS\nCatálogo: oms_cacaushow_prd\n\nString de Conexão:\nServer=*************=oms_cacaushow_prd;User ID=APIOMS;Password=***********;Max Pool Size=1000\n\nDirecionamento: Banco de dados de Produção do OMS.",
                    Db = "Server=*************=oms_cacaushow_prd;User ID=APIOMS;Password=***********;Max Pool Size=1000"
                }
            }
        },
        new NavNode
        {
            Id = "bancos-nao-relacionados", Label = "Bancos Não Relacionados", Icon = "🗄️", Pending = true,
            Children = new()
            {
                new NavNode
                {
                    Id = "master-retail", Label = "Master Retail / Ecossistema Trufa Negra", Icon = "📁",
                    Children = new()
                    {
                        new NavNode { Id = "trufanegra", Label = "trufaNegra",
                            Content = "Conexão: trufaNegra\nCatálogos: mr2_cacaushow e vw_trufanegra\n\nDirecionamento: Ecossistema Master Retail.",
                            Db = "trufaNegra (mr2_cacaushow / vw_trufanegra)" },
                        new NavNode { Id = "trufanegracd", Label = "trufaNegraCD",
                            Content = "Conexão: trufaNegraCD\nCatálogo: cacaudigital_web_prd\n\nDirecionamento: Fronteira de integração entre Master Retail e Cacau Digital.",
                            Db = "trufaNegraCD (cacaudigital_web_prd)" },
                        new NavNode { Id = "trufanegravd", Label = "trufaNegraVD",
                            Content = "Conexão: trufaNegraVD\nCatálogos: rcky_mr_dtw e mr2_cacaushow\n\nDirecionamento: Ecossistema Master Retail / Vendas Diretas.",
                            Db = "trufaNegraVD (rcky_mr_dtw / mr2_cacaushow)" },
                        new NavNode { Id = "trufanegrapanettone", Label = "trufaNegraPanettone",
                            Content = "Conexão: trufaNegraPanettone\nCatálogo: mr2_cacaushow\n\nDirecionamento: Ecossistema Master Retail.",
                            Db = "trufaNegraPanettone (mr2_cacaushow)" },
                        new NavNode { Id = "trufanegraecommerce", Label = "trufaNegraEcommerce",
                            Content = "Conexão: trufaNegraEcommerce\nCatálogo: mr2_cacaushow\n\nDirecionamento: Ecossistema Master Retail / E-commerce.",
                            Db = "trufaNegraEcommerce (mr2_cacaushow)" },
                        new NavNode { Id = "trufanegramm", Label = "trufaNegraMM",
                            Content = "Conexão: trufaNegraMM\nCatálogo: bematech_stg\n\nDirecionamento: Ecossistema Master Retail / Notas fiscais.",
                            Db = "trufaNegraMM (bematech_stg)" }
                    }
                },
                new NavNode
                {
                    Id = "corporativo-dw", Label = "Corporativo & Data Warehouse", Icon = "📁",
                    Children = new()
                    {
                        new NavNode { Id = "dw", Label = "DW",
                            Content = "Conexão: DW\nCatálogo: DW_CACAUSHOW\n\nDirecionamento: Data Warehouse central.",
                            Db = "DW (DW_CACAUSHOW)" },
                        new NavNode { Id = "bicc", Label = "BICC",
                            Content = "Conexão: BICC\nCatálogo: BICC_PRD\n\nDirecionamento: Business Intelligence / Controladoria.",
                            Db = "BICC (BICC_PRD)" },
                        new NavNode { Id = "protheus", Label = "Protheus",
                            Content = "Conexão: Protheus\nCatálogo: mp12_erp_prd\n\nDirecionamento: ERP Protheus TOTVS.",
                            Db = "Protheus (mp12_erp_prd)" },
                        new NavNode { Id = "lake", Label = "Lake",
                            Content = "Conexão: Lake\nCatálogo: STM\n\nDirecionamento: Data Lake corporativo.",
                            Db = "Lake (STM)" },
                        new NavNode { Id = "lakebip", Label = "LakeBIP",
                            Content = "Conexão: LakeBIP\nCatálogo: BIP\n\nDirecionamento: Data Lake BIP.",
                            Db = "LakeBIP (BIP)" },
                        new NavNode { Id = "mrd", Label = "MRD",
                            Content = "Conexão: MRD\nCatálogo: MR2_CACAUSHOW\n\nDirecionamento: A definir.",
                            Db = "MRD (MR2_CACAUSHOW)" }
                    }
                },
                new NavNode
                {
                    Id = "outros-classificar", Label = "Outros / A Classificar", Icon = "📁",
                    Children = new()
                    {
                        new NavNode { Id = "cde", Label = "CDE",
                            Content = "Conexão: CDE\nCatálogo: web_consultoria\n\nStatus: Pendente de direcionamento.",
                            Db = "CDE (web_consultoria)" },
                        new NavNode { Id = "multiloja", Label = "MultiLoja",
                            Content = "Conexão: MultiLoja\nCatálogo: CSDTB-RCKY-DTW\n\nStatus: Pendente de direcionamento.",
                            Db = "MultiLoja (CSDTB-RCKY-DTW)" },
                        new NavNode { Id = "nps", Label = "NPS",
                            Content = "Conexão: NPS\nCatálogo: nps2_sys_prd\n\nStatus: Sistema de pesquisas (NPS).",
                            Db = "NPS (nps2_sys_prd)" },
                        new NavNode { Id = "credenciamento", Label = "Credenciamento",
                            Content = "Conexão: Credenciamento\nCatálogo: rcky_mr_dtw\n\nStatus: Pendente de direcionamento.",
                            Db = "Credenciamento (rcky_mr_dtw)" },
                        new NavNode { Id = "sysaid", Label = "SYSAID",
                            Content = "Conexão: SYSAID\nCatálogo: sysaid_sys_prd\n\nStatus: Gestão de chamados (Sysaid).",
                            Db = "SYSAID (sysaid_sys_prd)" },
                        new NavNode { Id = "inventario-geral", Label = "Inventario",
                            Content = "Conexão: Inventario\nCatálogo: inventario\n\nStatus: Controle de Estoque/Inventário Cacau Digital.",
                            Db = "Inventario (inventario)" }
                    }
                }
            }
        },
        new NavNode
        {
            Id = "em-breve", Label = "Em breve",
            Children = new()
            {
                new NavNode
                {
                    Id = "api-pass", Label = "coletando informações",
                    Content = "Informações sobre a API pass.",
                    Db = "Api pass"
                }
            }
        }
    };
}
