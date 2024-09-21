export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPAges: number;
}

export class PaginatedResult<T> {
    items?: T;
    pagination?: Pagination
}