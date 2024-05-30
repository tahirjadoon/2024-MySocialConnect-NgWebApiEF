import { Pagination } from "./pagination.model";

export class PaginatedResult<T> {
    result?: T; //it can be optional. could be undefined as well with !
    pagination?: Pagination //it can be optional. could be undefined as well with !
}
