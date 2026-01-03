using System.Text;

namespace ExecuteSQLStatementsFromFIle
{
    internal class SplitSqlStatementsParent
    { // Splits by semicolons, but keeps semicolons inside:
      // - single quotes '...'
      // - double quotes "..."
      // - bracket identifiers [...]
      // - line comments -- ...
      // - block comments /* ... */
      //
      // NOTE: This is a pragmatic splitter, not a full SQL parser.
        internal static List<string> SplitSqlStatements(string sqlLine)
        {
            var result = new List<string>();
            var sb = new StringBuilder();

            bool inSingle = false;
            bool inDouble = false;
            bool inBracket = false;
            bool inLineComment = false;
            bool inBlockComment = false;

            for (int i = 0; i < sqlLine.Length; i++)
            {
                char c = sqlLine[i];
                char next = i + 1 < sqlLine.Length ? sqlLine[i + 1] : '\0';

                if (inLineComment)
                {
                    // Everything to end-of-line is comment
                    sb.Append(c);
                    continue;
                }

                if (inBlockComment)
                {
                    sb.Append(c);
                    if (c == '*' && next == '/')
                    {
                        sb.Append(next);
                        i++;
                        inBlockComment = false;
                    }
                    continue;
                }

                // Start comments (only if we're not inside quotes/identifiers)
                if (!inSingle && !inDouble && !inBracket)
                {
                    if (c == '-' && next == '-')
                    {
                        sb.Append(c);
                        sb.Append(next);
                        i++;
                        inLineComment = true;
                        continue;
                    }
                    if (c == '/' && next == '*')
                    {
                        sb.Append(c);
                        sb.Append(next);
                        i++;
                        inBlockComment = true;
                        continue;
                    }
                }

                // Toggle quote/identifier states
                if (!inDouble && !inBracket && c == '\'')
                {
                    // Handle escaped single quote '' inside single string
                    if (inSingle && next == '\'')
                    {
                        sb.Append(c);
                        sb.Append(next);
                        i++;
                        continue;
                    }
                    inSingle = !inSingle;
                    sb.Append(c);
                    continue;
                }

                if (!inSingle && !inBracket && c == '"')
                {
                    // Handle escaped double quote "" inside double-quoted identifier/string
                    if (inDouble && next == '"')
                    {
                        sb.Append(c);
                        sb.Append(next);
                        i++;
                        continue;
                    }
                    inDouble = !inDouble;
                    sb.Append(c);
                    continue;
                }

                if (!inSingle && !inDouble && c == '[')
                {
                    inBracket = true;
                    sb.Append(c);
                    continue;
                }

                if (inBracket && c == ']')
                {
                    inBracket = false;
                    sb.Append(c);
                    continue;
                }

                // Statement delimiter
                if (!inSingle && !inDouble && !inBracket && c == ';')
                {
                    var stmt = sb.ToString().Trim();
                    if (stmt.Length > 0)
                        result.Add(stmt);

                    sb.Clear();
                    // don't include the semicolon
                    continue;
                }

                sb.Append(c);
            }

            var last = sb.ToString().Trim();
            if (last.Length > 0)
                result.Add(last);

            return result;
        }
        
    }
}
