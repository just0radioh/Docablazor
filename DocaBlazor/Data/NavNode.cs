namespace DocaBlazor.Data;

/// <summary>
/// Nó da árvore de navegação. Pode ser uma pasta (quando <see cref="Children"/>
/// não é nulo) ou um item de conteúdo (quando <see cref="Content"/> não é nulo).
/// </summary>
public class NavNode
{
    public string Id { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public bool Pending { get; init; }

    /// <summary>Texto exibido no card de conteúdo. Nulo quando o nó é uma pasta.</summary>
    public string? Content { get; init; }

    /// <summary>Informação de banco/conexão exibida no rodapé. Nulo quando o nó é uma pasta.</summary>
    public string? Db { get; init; }

    public List<NavNode>? Children { get; init; }

    public bool IsFolder => Children is { Count: > 0 };

    /// <summary>Preenchido em tempo de execução pelo <see cref="DocsRepository"/>.</summary>
    public NavNode? Parent { get; set; }

    public bool MatchesQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return true;
        var q = query.Trim();

        if (Label.Contains(q, StringComparison.OrdinalIgnoreCase)) return true;
        if (Content is not null && Content.Contains(q, StringComparison.OrdinalIgnoreCase)) return true;
        if (Children is not null) return Children.Any(c => c.MatchesQuery(q));
        return false;
    }

    public string Breadcrumb()
    {
        var parts = new List<string> { Label };
        var p = Parent;
        while (p is not null)
        {
            parts.Insert(0, p.Label);
            p = p.Parent;
        }
        return string.Join(" › ", parts).ToUpperInvariant();
    }
}
