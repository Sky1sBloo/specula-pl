#include "OperatorStateManager.hpp"

OperatorStateManager::OperatorStateManager()
    : currentState(State::START)
{
}

// Used for manipulating state based on char
void OperatorStateManager::addChar(char c)
{
    switch (currentState) {
    case State::START:
        handleStartState(c);
        break;
    case State::EQUALS_NEXT:
        handleEqualsNext(c);
        break;
    case State::ADD:
        handleAdd(c);
        break;
    case State::SUB:
        handleSub(c);
        break;
    case State::END:
        break;
    case State::INVALID:
        break;
    }
}

void OperatorStateManager::handleStartState(char c)
{
    switch (c) {
    case '=':
    case '*':
    case '/':
    case '%':
        currentState = State::EQUALS_NEXT;
        break;
    case '+':
        currentState = State::ADD;
        break;
    case '-':
        currentState = State::SUB;
        break;
    default:
        currentState = State::INVALID;
    }
}

void OperatorStateManager::handleEqualsNext(char c)
{
    switch (c) {
    case '=':
        currentState = State::END;
        break;
    default:
        currentState = State::INVALID;
    }
}

void OperatorStateManager::handleAdd(char c)
{
    switch (c) {
    case '=':
    case '+':
        currentState = State::END;
        break;
    }
}
void OperatorStateManager::handleSub(char c)
{
    switch (c) {
    case '=':
    case '-':
        currentState = State::END;
        break;
    }
}
