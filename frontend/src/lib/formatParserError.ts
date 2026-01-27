export interface ParsedError {
    line?: number;
    column?: number;
    received?: string;
    expected?: string[];
    raw: string;
}

/**
 * Parse a parser error string to extract structured information
 * 
 * Example input: "Expected token: ;, Received: K_STATE. at 5:5"
 * Returns: { line: 5, column: 5, received: "K_STATE", expected: [";"], raw: "..." }
 */
export function parseErrorMessage(errorString: string): ParsedError {
    const result: ParsedError = {
        raw: errorString
    };

    // Extract line and column from "at LINE:COLUMN" pattern
    const locationMatch = errorString.match(/\bat\s+(\d+):(\d+)/);
    if (locationMatch) {
        result.line = parseInt(locationMatch[1], 10);
        result.column = parseInt(locationMatch[2], 10);
    }

    // Extract received/unexpected token
    const receivedMatch = errorString.match(/Received:\s*([^.\s,]+)/);
    if (receivedMatch) {
        result.received = receivedMatch[1];
    }

    // Extract expected tokens (can be comma-separated list)
    const expectedMatch = errorString.match(/Expected token:\s*([^,]+?)(?:,\s*Received:|$)/);
    if (expectedMatch) {
        const expectedStr = expectedMatch[1].trim();
        // Split by commas but keep tokens together
        result.expected = expectedStr.split(',').map(s => s.trim()).filter(s => s.length > 0);
    } else {
        // Try alternative pattern for multiple expected tokens
        const multiExpectedMatch = errorString.match(/Expected token:\s*(.+?)\s*Received:/);
        if (multiExpectedMatch) {
            const expectedStr = multiExpectedMatch[1].trim();
            result.expected = expectedStr.split(',').map(s => s.trim()).filter(s => s.length > 0);
        }
    }

    return result;
}

/**
 * Format a parsed error for display in the tree view
 */
export function formatErrorForTree(parsed: ParsedError): string {
    const parts: string[] = [];
    
    // Extract the message part (everything before "at LINE:COLUMN")
    const messageMatch = parsed.raw.match(/^(.+?)\s+at\s+\d+:\d+/);
    const messageText = messageMatch ? messageMatch[1].trim() : null;

    if (parsed.line !== undefined && parsed.column !== undefined) {
        parts.push(`Line ${parsed.line}, Column ${parsed.column}`);
    }
    
    // Add the main error message if it exists
    if (messageText && !parsed.received && !parsed.expected) {
        parts.push(messageText);
    }

    if (parsed.received) {
        parts.push(`Unexpected: ${parsed.received}`);
    }

    if (parsed.expected && parsed.expected.length > 0) {
        if (parsed.expected.length === 1) {
            parts.push(`Expected: ${parsed.expected[0]}`);
        } else {
            parts.push(`Expected one of: ${parsed.expected.join(', ')}`);
        }
    }

    return parts.length > 0 ? parts.join(' | ') : parsed.raw;
}

/**
 * Format a parsed error for display in the dropdown view
 */
export function formatErrorForDropdown(parsed: ParsedError): string {
    const parts: string[] = [];

    if (parsed.line !== undefined && parsed.column !== undefined) {
        parts.push(`at ${parsed.line}:${parsed.column}`);
    }

    if (parsed.received) {
        parts.push(`got ${parsed.received}`);
    }

    if (parsed.expected && parsed.expected.length > 0) {
        if (parsed.expected.length === 1) {
            parts.push(`expected ${parsed.expected[0]}`);
        } else {
            parts.push(`expected ${parsed.expected.join(' or ')}`);
        }
    }

    return parts.length > 0 ? parts.join(', ') : parsed.raw;
}
