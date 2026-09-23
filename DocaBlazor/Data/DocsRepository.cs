using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DocaBlazor.Data;

/// <summary>
/// Repositório com a árvore padrão da base de conhecimento e helpers de CRUD/serialização.
/// O conteúdo padrão vem de wwwroot/data/nav-tree.json (equivalente ao DEFAULT_NAV_TREE do HTML).
/// </summary>
public class DocsRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public const string StorageKey = "DOCA_NAV_TREE_V2";
    public const string DefaultDataRelativePath = "data/nav-tree.json";

    private static readonly Regex SlugInvalidChars = new(
        "[^a-z0-9]+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private readonly IWebHostEnvironment _env;
    private readonly ILogger<DocsRepository> _logger;
    private List<NavNode>? _defaultTemplate;

    public DocsRepository(IWebHostEnvironment env, ILogger<DocsRepository> logger)
    {
        _env = env;
        _logger = logger;
    }

    /// <summary>Retorna uma cópia profunda da árvore padrão.</summary>
    public List<NavNode> CreateDefaultTree()
    {
        _defaultTemplate ??= LoadDefaultFromFile() ?? FallbackTree();
        var clone = _defaultTemplate.Select(n => n.DeepClone()).ToList();
        LinkParents(clone, null);
        return clone;
    }

    private List<NavNode>? LoadDefaultFromFile()
    {
        if (string.IsNullOrEmpty(_env.WebRootPath))
        {
            _logger.LogWarning("WebRootPath não configurado; usando árvore de fallback.");
            return null;
        }

        var path = Path.Combine(_env.WebRootPath, "data", "nav-tree.json");
        if (!File.Exists(path))
        {
            _logger.LogWarning("Arquivo padrão não encontrado em {Path}.", path);
            return null;
        }

        try
        {
            var json = File.ReadAllText(path);
            var tree = Deserialize(json);
            if (tree is null)
                _logger.LogWarning("Não foi possível interpretar {Path}.", path);
            return tree;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao carregar {Path}.", path);
            return null;
        }
    }

    public static void LinkParents(IEnumerable<NavNode> nodes, NavNode? parent)
    {
        foreach (var node in nodes)
        {
            node.Parent = parent;
            if (node.Children is not null)
                LinkParents(node.Children, node);
        }
    }

    public static string Serialize(List<NavNode> tree) =>
        JsonSerializer.Serialize(tree, JsonOptions);

    public static List<NavNode>? Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "[]") return null;
        try
        {
            var tree = JsonSerializer.Deserialize<List<NavNode>>(json, JsonOptions);
            if (tree is null || tree.Count == 0) return null;
            Normalize(tree);
            LinkParents(tree, null);
            return tree;
        }
        catch
        {
            return null;
        }
    }

    public static string Slugify(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "item";

        var decomposed = name.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                builder.Append(c);
        }

        var slug = builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        try
        {
            slug = SlugInvalidChars.Replace(slug, "-").Trim('-');
        }
        catch (RegexMatchTimeoutException)
        {
            return "item";
        }

        return string.IsNullOrEmpty(slug) ? "item" : slug;
    }

    public static string UniqueId(string baseName, IEnumerable<NavNode>? existing = null)
    {
        var slug = Slugify(baseName);
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var id = $"{slug}-{Guid.NewGuid().ToString("N")[..8]}";
            if (existing is null || FindById(existing, id) is null)
                return id;
        }

        return $"{slug}-{Guid.NewGuid():N}";
    }

    /// <summary>Garante rótulos, remove nulos e evita ids vazios ou duplicados.</summary>
    public static void Normalize(List<NavNode> roots)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        NormalizeNodes(roots, seen);
    }

    private static void NormalizeNodes(List<NavNode> nodes, HashSet<string> seen)
    {
        nodes.RemoveAll(n => n is null);
        foreach (var node in nodes)
        {
            node.Label ??= string.Empty;
            if (string.IsNullOrWhiteSpace(node.Id) || !seen.Add(node.Id))
                node.Id = $"{Slugify(node.Label)}-{Guid.NewGuid():N}";

            if (node.Children is null) continue;
            NormalizeNodes(node.Children, seen);
        }
    }

    public static NavNode? FindById(IEnumerable<NavNode> nodes, string id)
    {
        foreach (var n in nodes)
        {
            if (n.Id == id) return n;
            if (n.Children is not null)
            {
                var found = FindById(n.Children, id);
                if (found is not null) return found;
            }
        }
        return null;
    }

    public static bool RemoveById(List<NavNode> nodes, string id)
    {
        var idx = nodes.FindIndex(n => n.Id == id);
        if (idx >= 0)
        {
            nodes.RemoveAt(idx);
            return true;
        }

        foreach (var n in nodes)
        {
            if (n.Children is not null && RemoveById(n.Children, id))
                return true;
        }
        return false;
    }

    /// <summary>Fallback mínimo caso o JSON não exista no disco.</summary>
    private static List<NavNode> FallbackTree() => new()
    {
        new NavNode
        {
            Id = "em-breve",
            Label = "Em breve",
            Children = new()
            {
                new NavNode
                {
                    Id = "api-pass",
                    Label = "coletando informações",
                    Content = "Informações sobre a API pass. Arquivo data/nav-tree.json não encontrado.",
                    Db = "Api pass"
                }
            }
        }
    };
}
