/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { ResultCode } from "./ResultCode";

export interface ApiResponse<T> {
    response: T;
    code: ResultCode;
    errorMessage: string;
    isSuccess: boolean;
}
