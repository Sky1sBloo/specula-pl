#pragma once

#include <array>
#include <optional>
#include <string>
#include <string_view>
#include <unordered_map>
#include <vector>

#include "ErrorLines.hpp"
#include "Tokens.hpp"

struct Token {
    TokenType type;
    std::string value;
};

enum class LexerState {
    START,
    DELIMETER,
    EXPECT_DELIMETER, // next character must be a delimeter
    IDENTIFIER,
    IDENTIFIER_DASH,
    NUM_START,
    DECIMAL_REACHED,
    FLOAT,
    CHAR_START,
    CHAR_END,
    CHAR_ESCAPE_CHAR,
    STRING_START,
    STRING,
    STRING_ESCAPE_CHAR,
    OP,
    OP_EQUALS_NEXT,
    OP_INCREMENTABLE,
    OP_LOGICAL,
    CHAR_SLASH,
    OP_MINUS,
    OP_LESS_THAN,
    OP_GREATER_THAN,
    OP_LEFT_ARROW,
    COMMENT,
    MULTILINE_COMMENT,
    MULTILINE_COMMENT_END,
    INVALID
};

/**
 * Main class for handling lexical tokens
 */
class LexicalAnalyzer {
public:
    /**
     * Default constructor
     */
    LexicalAnalyzer();

    /**
     * Constructor but also build tokens
     */
    LexicalAnalyzer(std::string_view text);

    /**
     * Clears state and tokens
     */
    void reset();

    /**
     * Setups tokens based on the text
     *
     * @param text Text to parse
     * @param line Used for error handling
     *
     * @throws LexerError
     */
    void buildTokens(std::string_view text, int line = -1);

    /**
     * Gets the tokens from the processed string
     */
    const std::vector<Token>& getTokens() const { return mTokens; }

private:
    LexerState mCurrentState;
    char mToRead;
    std::string mLexeme; // to be appended by build tokens

    int mLine;
    int mCharPos;

    std::vector<Token> mTokens;
    std::vector<ErrorLines> mErrors;

    static const std::unordered_map<std::string_view, TokenType> mOperators;
    static const std::unordered_map<char, TokenType> mDelimeters;
    static const std::unordered_map<std::string_view, TokenType> mKeywords;
    static constexpr std::array<char, 2> mForceStringEscape = { '\n', '\r' }; // characters that force string to terminate
    static constexpr std::array<char, 11> escapeChar = { '\'', '"', '\\', '?', 'a', 'b', 'f', 'n', 'r', 't', 'v' };

    enum class HandleStateResult {
        CONTINUE,
        REPROCESS // When handleState doesn't store the character to the lexeme (mainly for exiting states)
    };

    // Calls respective state functions
    HandleStateResult handleState();

    void resetState();
    // Calls when no string is being read
    void flushLeftoverLexeme();

    // State functions
    HandleStateResult handleStartState();
    HandleStateResult handleInvalidState();
    HandleStateResult handleDelimeterState();
    HandleStateResult handleExpectDelimeterState();
    HandleStateResult handleIdentifierState();
    HandleStateResult handleIdentifierDashState();

    HandleStateResult handleNumStartState();
    HandleStateResult handleDecimalState();
    HandleStateResult handleFloatState();
    HandleStateResult handleCharStartState();
    HandleStateResult handleCharEndState();
    HandleStateResult handleCharEscapeCharState();
    HandleStateResult handleStringState();
    HandleStateResult handleStringEscapeCharState();

    HandleStateResult handleOpState();
    HandleStateResult handleOpEqualsNextState();
    HandleStateResult handleIncrementableState();
    HandleStateResult handleOpLogicalState();
    HandleStateResult handleOpMinusState();
    HandleStateResult handleOpLessThanState();
    HandleStateResult handleOpGreaterThanState();
    HandleStateResult handleOpLeftArrowState();

    HandleStateResult handleCharSlashState();
    HandleStateResult handleCommentState();
    HandleStateResult handleMultilineCommentState();
    HandleStateResult handleMultilineCommentEndState();

    /// Helper functions
    // Saves contents from mLexeme to mTokens
    void saveToken(TokenType type);

    // Used for throwing an error
    HandleStateResult setStateInvalid(std::string message);

    bool isValidIdentifier(char c);
    void finalizeIdentifier();
    void finalizeIdentifierDash();

    bool isValidOperator(char c);
    LexerState getOperatorStartState(char c);
    TokenType getSingleOperatorToken(char c);

    // Appends backslash before it
    char charToEscapeChar(char c);

    std::optional<TokenType> getDelimeter(char c);
    std::optional<TokenType> getKeyword(std::string_view value);
};
