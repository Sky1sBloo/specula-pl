#pragma once

#include "Tokens.hpp"

class OperatorStateManager {
public:
    enum class State {
        START,
        EQUALS_NEXT,
        ADD,
        SUB,
        END,
        INVALID
    };

    OperatorStateManager();

    // Used for manipulating state based on char
    void addChar(char c);

    TokenType getTokenType() const;

    const State& getState() const { return currentState; }
    bool isValidEndState() const;

private:
    State currentState;

    void handleStartState(char c);
    void handleEqualsNext(char c);
    void handleAdd(char c);
    void handleSub(char c);
};
