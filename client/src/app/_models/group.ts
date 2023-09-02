export interface Group{
    name: string;
    connections: Connection[];
}

export interface Connection{
    connectionI: string;
    username: string;
}