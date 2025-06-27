-- Consultas Básicas
-- 1. Selecionar todos os produtos
SELECT * FROM Produtos;

-- 2. Contar o número total de produtos
SELECT COUNT(*) AS TotalProdutos FROM Produtos;

-- 3. Listar produtos ordenados por preço (do mais caro para o mais barato)
SELECT Id, Nome, Preco, QuantidadeEmEstoque 
FROM Produtos 
ORDER BY Preco DESC;

-- 4. Produtos com estoque baixo (menos de 10 itens)
SELECT Id, Nome, QuantidadeEmEstoque 
FROM Produtos 
WHERE QuantidadeEmEstoque < 10 
ORDER BY QuantidadeEmEstoque;

--Consultas com Agregações
-- 5. Média de preço dos produtos
SELECT AVG(Preco) AS MediaPreco FROM Produtos;

-- 6. Valor total em estoque (soma de preço * quantidade)
SELECT SUM(Preco * QuantidadeEmEstoque) AS ValorTotalEmEstoque 
FROM Produtos;

-- 7. Produto mais caro
SELECT TOP 1 Nome, Preco 
FROM Produtos 
ORDER BY Preco DESC;

-- 8. Produtos agrupados por faixa de preço
SELECT 
    CASE 
        WHEN Preco < 50 THEN 'Até R$ 50,00'
        WHEN Preco BETWEEN 50 AND 100 THEN 'R$ 50,00 a R$ 100,00'
        ELSE 'Acima de R$ 100,00'
    END AS FaixaPreco,
    COUNT(*) AS Quantidade
FROM Produtos
GROUP BY 
    CASE 
        WHEN Preco < 50 THEN 'Até R$ 50,00'
        WHEN Preco BETWEEN 50 AND 100 THEN 'R$ 50,00 a R$ 100,00'
        ELSE 'Acima de R$ 100,00'
    END;

-- Consultas com Datas
-- 9. Produtos adicionados nos últimos 30 dias
SELECT Nome, DataCriacao 
FROM Produtos 
WHERE DataCriacao >= DATEADD(day, -30, GETDATE())
ORDER BY DataCriacao DESC;

-- 10. Produtos atualizados recentemente
SELECT Nome, DataAtualizacao 
FROM Produtos 
WHERE DataAtualizacao IS NOT NULL
ORDER BY DataAtualizacao DESC;

-- 11. Produtos agrupados por mês de criação
SELECT 
    FORMAT(DataCriacao, 'yyyy-MM') AS MesAno,
    COUNT(*) AS TotalProdutos
FROM Produtos
GROUP BY FORMAT(DataCriacao, 'yyyy-MM')
ORDER BY MesAno;

--Consultas de Atualização
-- 12. Atualizar preço de todos os produtos (aumento de 10%)
UPDATE Produtos 
SET Preco = Preco * 1.10,
    DataAtualizacao = GETDATE()
WHERE QuantidadeEmEstoque > 0;

-- 13. Atualizar estoque após venda
UPDATE Produtos 
SET QuantidadeEmEstoque = QuantidadeEmEstoque - 1,
    DataAtualizacao = GETDATE()
WHERE Id = 1 AND QuantidadeEmEstoque > 0;

-- 14. Atualizar descrição de um produto específico
UPDATE Produtos 
SET Descricao = 'Nova descrição mais detalhada',
    DataAtualizacao = GETDATE()
WHERE Id = 1;

--Consultas de Exclusão
-- 15. Remover produtos sem estoque
DELETE FROM Produtos 
WHERE QuantidadeEmEstoque = 0;

-- 16. Remover produtos antigos (criados há mais de 1 ano)
DELETE FROM Produtos 
WHERE DataCriacao < DATEADD(year, -1, GETDATE());

--Consultas Avançadas
-- 17. Produtos com nome que contém uma palavra específica
SELECT * FROM Produtos 
WHERE Nome LIKE '%notebook%' 
   OR Descricao LIKE '%notebook%';

-- 18. Produtos ordenados por margem de lucro (assumindo um custo fixo de 70% do preço)
SELECT 
    Nome, 
    Preco,
    Preco * 0.7 AS CustoEstimado,
    Preco * 0.3 AS LucroEstimado,
    (Preco * 0.3) / (Preco * 0.7) * 100 AS MargemPercentual
FROM Produtos
ORDER BY MargemPercentual DESC;

-- 19. Produtos que precisam de reposição (estoque abaixo de 5% da média)
WITH MediaEstoque AS (
    SELECT AVG(QuantidadeEmEstoque) AS Media
    FROM Produtos
)
SELECT 
    p.Id,
    p.Nome,
    p.QuantidadeEmEstoque,
    m.Media AS MediaEstoqueGeral
FROM Produtos p
CROSS JOIN MediaEstoque m
WHERE p.QuantidadeEmEstoque < (m.Media * 0.05);