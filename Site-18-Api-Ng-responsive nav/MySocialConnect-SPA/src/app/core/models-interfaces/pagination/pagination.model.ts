export class Pagination {
    constructor(public currentPage: number, 
                public totalPages: number, 
                public itemsPerPage: number, 
                public totalItems: number){}
}