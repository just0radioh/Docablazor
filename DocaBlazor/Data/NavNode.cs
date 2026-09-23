using System.Text.Json.Serialization;

namespace DocaBlazor.Data;

/// <summary>
/// Nó da árvore de navegação. Pode ser uma pasta (quando <see cref="Children"/>
/// não é nulo) ou um item de conteúdo (quando <see cref="Content"/> não é nulo).
/// </summary>
public class NavNode
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public bool Pending { get; set; }

    /// <summary>Texto exibido no card de conteúdo. Nulo quando o nó é uma pasta.</summary>
    public string? Content { get; set; }

    /// <summary>Informação de banco/conexão exibida no rodapé. Nulo quando o nó é uma pasta.</summary>
    public string? Db { get; set; }

    /// <summary>Quantidade de vezes que a página foi editada e salva.</summary>
    public int Acessos { get; set; }

    public List<NavNode>? Children { get; set; }

    [JsonIgnore]
    public bool IsFolder => Children is not null;

    [JsonIgnore]
    public bool IsPage => Content is not null;

    /// <summary>Preenchido em tempo de execução pelo <see cref="DocsRepository"/>.</summary>
    [JsonIgnore]
    public NavNode? Parent { get; set; }

    public bool MatchesQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return true;
        var q = query.Trim();

        if ((Label ?? string.Empty).Contains(q, StringComparison.OrdinalIgnoreCase)) return true;
        if (Content is not null && Content.Contains(q, StringComparison.OrdinalIgnoreCase)) return true;
        if (Children is not null) return Children.Any(c => c.MatchesQuery(q));
        return false;
    }

    public string Breadcrumb()
    {
        var parts = new List<string> { Label ?? string.Empty };
        var p = Parent;
        while (p is not null)
        {
            parts.Insert(0, p.Label ?? string.Empty);
            p = p.Parent;
        }
        return string.Join(" › ", parts).ToUpperInvariant();
    }

    public NavNode DeepClone()
    {
        return new NavNode
        {
            Id = Id,
            Label = Label,
            Icon = Icon,
            Pending = Pending,
            Content = Content,
            Db = Db,
            Acessos = Acessos,
            Children = Children?.Select(c => c.DeepClone()).ToList()
        };
    }
}
