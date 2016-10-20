namespace Inceptum.Sprache.Binary
{
    /// <summary>
    /// Parser Delegate
    /// </summary>
    /// <typeparam name="T">Type of parsed result</typeparam>
    /// <param name="input">input</param>
    /// <returns></returns>
    public delegate IResult<T> Parser<out T>(IInput input);
}