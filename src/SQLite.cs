using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SQLite
{
    public static class API
    {
        // Change this if needed
        public const string DLL_PATH = "sqlite3.dll";

        public const int SQLITE_OK = 0;
        public const int SQLITE_ERROR = 1;
        public const int SQLITE_BUSY = 5;
        public const int SQLITE_TOOBIG = 18;
        public const int SQLITE_MISUSE = 21;
        public const int SQLITE_DONE = 101;
        public const int SQLITE_ROW = 100;

        public const int SQLITE_INTEGER = 1;
        public const int SQLITE_FLOAT = 2;
        public const int SQLITE_TEXT = 3;
        public const int SQLITE_BLOB = 4;
        public const int SQLITE_NULL = 5;

        // Make a copy of the blobs/strings that are passed to sqlite3_bind_*
        // It's inefficient, but it's easier than managing the raw IntPtr of the buffer
        // in order to make sure that it doesn't get deallocated too early.
        // This SQLite wrapper aims for simplicity. If we needed true performance,
        // we would've used C/C++ or written a more complex wrapper.
        public static readonly IntPtr SQLITE_TRANSISTENT = (IntPtr)(-1);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_open", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_open([MarshalAs(UnmanagedType.LPStr)] string filename, ref IntPtr ppDb);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_close_v2", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_close_v2(IntPtr ptr);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_free", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_free(IntPtr ptr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int Callback(IntPtr context, int columns, IntPtr text, IntPtr names);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_exec", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_exec(IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string sql, Callback callback, IntPtr context, ref IntPtr errmsg);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_prepare_v2", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_prepare_v2(IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string zSql, int nByte, ref IntPtr ppStmt, ref IntPtr pzTail);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_reset", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_reset(IntPtr pStmt);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_step(IntPtr pStmt);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_bind_blob", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_bind_blob(IntPtr pStmt, int index, IntPtr data, int n, IntPtr destructor);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_bind_double", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_bind_double(IntPtr pStmt, int index, double data);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_bind_int(IntPtr pStmt, int index, int data);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_bind_int64(IntPtr pStmt, int index, long data);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_bind_null", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_bind_null(IntPtr pStmt, int index);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_bind_text", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_bind_text(IntPtr pStmt, int index, [MarshalAs(UnmanagedType.LPStr)] string data, int n, IntPtr destructor);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_finalize", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_finalize(IntPtr pStmt);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_column_count", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_column_count(IntPtr pStmt);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_column_name", CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr sqlite3_column_name(IntPtr pStmt, int N);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_column_type", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_column_type(IntPtr pStmt, int iCol);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_column_int", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_column_int(IntPtr pStmt, int iCol);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_column_int64", CallingConvention = CallingConvention.Cdecl)]
        public extern static long sqlite3_column_int64(IntPtr pStmt, int iCol);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_column_double", CallingConvention = CallingConvention.Cdecl)]
        public extern static double sqlite3_column_double(IntPtr pStmt, int iCol);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_column_text", CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr sqlite3_column_text(IntPtr pStmt, int iCol);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_column_blob", CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr sqlite3_column_blob(IntPtr pStmt, int iCol);

        [DllImport(DLL_PATH, EntryPoint = "sqlite3_column_bytes", CallingConvention = CallingConvention.Cdecl)]
        public extern static int sqlite3_column_bytes(IntPtr pStmt, int iCol);
    }

    public class DBException : Exception
    {
        private int status;

        public DBException(int status)
        {
            this.status = status;
        }

        public override string ToString()
        {
            switch(status)
            {
                case API.SQLITE_BUSY:
                    return string.Format("Database Error (Status {0}): database file could not be read or written because of concurrent activity by some other database connection", status);
                default:
                    return string.Format("Database Error (Status {0}): unknown SQLite error", status);
            }
        }
    }

    public class QueryException : Exception
    {
        private int status;
        private string error;

        public QueryException(int status, string error)
        {
            this.status = status;
            this.error = error;
        }

        public override string ToString()
        {
            return string.Format("Query Error (Status {0}): {1}", status, !string.IsNullOrWhiteSpace(error) ? error : "unknown SQLite error");
        }
    }

    public class StatementException : Exception
    {
        private int status;

        public StatementException(int status)
        {
            this.status = status;
        }

        public override string ToString()
        {
            switch (status)
            {
                case API.SQLITE_TOOBIG:
                    return string.Format("Statement Error (Status {0}): query, string or blob is too large", status);
                case API.SQLITE_MISUSE:
                    return string.Format("Statement Error (Status {0}): interface was used in an undefined manner", status);
                default:
                    return string.Format("Statement Error (Status {0}): unknown SQLite error", status);
            }
        }
    }

    /// <summary>
    /// Represents a column containing a name and an array of values.
    /// </summary>
    public class Column
    {
        public enum Type
        {
            INTEGER = API.SQLITE_INTEGER,
            FLOAT = API.SQLITE_FLOAT,
            TEXT = API.SQLITE_TEXT,
            BLOB = API.SQLITE_BLOB,
            NULL = API.SQLITE_NULL
        }

        /// <summary>
        /// The type of the column, which should be taken into consideration when casting values.
        /// </summary>
        public Type type;

        public string name;
        public List<object> values;

        public Column()
        {
            name = "";
            values = new List<object>();
        }
    }

    /// <summary>
    /// Represents an SQLite database connection.
    /// </summary>
    public class DB
    {
        private IntPtr handle;

        /// <summary>
        /// Creates an SQLite database connection for a given file.
        /// </summary>
        public DB(string file)
        {
            handle = IntPtr.Zero;

            int status = API.sqlite3_open(file, ref handle);

            if(status != API.SQLITE_OK)
            {
                throw new DBException(status);
            }
        }

        /// <summary>
        /// Creates a statement from a query, which can be executed or have parameters bound to it.
        /// </summary>
        public Statement CreateStatement(string query)
        {
            return new Statement(handle, query);
        }

        public void Destroy()
        {
            if(handle == IntPtr.Zero)
            {
                return;
            }

            // close() can return an error status code, but since it's called from a destructor
            // there's not much we can do
            API.sqlite3_close_v2(handle);

            handle = IntPtr.Zero;
        }

        ~DB()
        {
            Destroy();
        }

        /// <summary>
        /// Represents an SQLite statement.
        /// </summary>
        public class Statement
        {
            private IntPtr handle;
            private bool shouldReset;

            public string query;

            public Statement(IntPtr dbHandle, string query)
            {
                this.query = query;

                IntPtr pzTail = IntPtr.Zero;

                int status = API.sqlite3_prepare_v2(dbHandle, query, query.Length, ref handle, ref pzTail);

                if(status != API.SQLITE_OK)
                {
                    throw new StatementException(status);
                }

                shouldReset = false;
            }

            /// <summary>
            /// Binds a BLOB to the parameter at the given index (starting from 1).
            /// </summary>
            public void BindBlob(int index, byte[] data)
            {
                GCHandle pinned = GCHandle.Alloc(data, GCHandleType.Pinned);
                IntPtr ptr = pinned.AddrOfPinnedObject();

                int status = API.sqlite3_bind_blob(handle, index, ptr, data.Length, API.SQLITE_TRANSISTENT);

                pinned.Free();

                if(status != API.SQLITE_OK)
                {
                    throw new StatementException(status);
                }
            }

            /// <summary>
            /// Binds a float to the parameter at the given index (starting from 1).
            /// </summary>
            public void BindFloat(int index, double data)
            {
                int status = API.sqlite3_bind_double(handle, index, data);

                if (status != API.SQLITE_OK)
                {
                    throw new StatementException(status);
                }
            }

            /// <summary>
            /// Binds an int to the parameter at the given index (starting from 1).
            /// </summary>
            public void BindInt(int index, int data)
            {
                int status = API.sqlite3_bind_int(handle, index, data);

                if (status != API.SQLITE_OK)
                {
                    throw new StatementException(status);
                }
            }

            /// <summary>
            /// Binds a long to the parameter at the given index (starting from 1).
            /// </summary>
            public void BindLong(int index, long data)
            {
                int status = API.sqlite3_bind_int64(handle, index, data);

                if (status != API.SQLITE_OK)
                {
                    throw new StatementException(status);
                }
            }

            /// <summary>
            /// Binds null to the parameter at the given index (starting from 1).
            /// </summary>
            public void BindNull(int index)
            {
                int status = API.sqlite3_bind_null(handle, index);

                if (status != API.SQLITE_OK)
                {
                    throw new StatementException(status);
                }
            }

            /// <summary>
            /// Binds a string to the parameter at the given index (starting from 1).
            /// </summary>
            public void BindString(int index, string data)
            {
                int status = API.sqlite3_bind_text(handle, index, data, data.Length, API.SQLITE_TRANSISTENT);

                if (status != API.SQLITE_OK)
                {
                    throw new StatementException(status);
                }
            }

            /// <summary>
            /// Executes the statement and returns an array of columns, each with a name and multiple values.
            /// All columns have the same number of values, which is also the number of rows.
            /// </summary>
            public Column[] Exec()
            {
                if(shouldReset)
                {
                    int resetStatus = API.sqlite3_reset(handle);

                    if(resetStatus != API.SQLITE_OK)
                    {
                        throw new StatementException(resetStatus);
                    }
                }
                else
                {
                    shouldReset = true;
                }

                int status = API.sqlite3_step(handle);

                switch(status)
                {
                    case API.SQLITE_DONE:
                        return null;
                    case API.SQLITE_ROW:
                        break;
                    default:
                        throw new StatementException(status);
                }

                int columnCount = API.sqlite3_column_count(handle);

                Column[] result = new Column[columnCount];

                for(int i = 0; i < columnCount; ++i)
                {
                    result[i] = new Column();

                    IntPtr rawName = API.sqlite3_column_name(handle, i);
                    result[i].name = Marshal.PtrToStringAnsi(rawName);

                    int type = API.sqlite3_column_type(handle, i);

                    switch (type)
                    {
                        case API.SQLITE_INTEGER:
                            result[i].type = Column.Type.INTEGER;
                            break;
                        case API.SQLITE_FLOAT:
                            result[i].type = Column.Type.FLOAT;
                            break;
                        case API.SQLITE_TEXT:
                            result[i].type = Column.Type.TEXT;
                            break;
                        case API.SQLITE_BLOB:
                            result[i].type = Column.Type.BLOB;
                            break;
                        case API.SQLITE_NULL:
                            result[i].type = Column.Type.NULL;
                            break;
                    }
                }

                do
                {
                    for(int i = 0; i < columnCount; ++i)
                    {
                        // If the column is null now, it may not be null in future rows
                        // Therefore, get the actual type
                        if(result[i].type == Column.Type.NULL)
                        {
                            int type = API.sqlite3_column_type(handle, i);

                            switch (type)
                            {
                                case API.SQLITE_INTEGER:
                                    result[i].type = Column.Type.INTEGER;
                                    break;
                                case API.SQLITE_FLOAT:
                                    result[i].type = Column.Type.FLOAT;
                                    break;
                                case API.SQLITE_TEXT:
                                    result[i].type = Column.Type.TEXT;
                                    break;
                                case API.SQLITE_BLOB:
                                    result[i].type = Column.Type.BLOB;
                                    break;
                                case API.SQLITE_NULL:
                                    break;
                            }
                        }

                        switch(result[i].type)
                        {
                            case Column.Type.INTEGER:
                            {
                                result[i].values.Add(API.sqlite3_column_int64(handle, i));
                                break;
                            }
                            case Column.Type.FLOAT:
                            {
                                result[i].values.Add(API.sqlite3_column_double(handle, i));
                                break;
                            }
                            case Column.Type.TEXT:
                            {
                                IntPtr ptr = API.sqlite3_column_text(handle, i);
                                int size = API.sqlite3_column_bytes(handle, i);

                                if(ptr == IntPtr.Zero)
                                {
                                    result[i].values.Add(null);
                                }
                                else
                                {
                                    result[i].values.Add(Marshal.PtrToStringAnsi(ptr, size));
                                }
                                break;
                            }
                            case Column.Type.BLOB:
                            {
                                IntPtr ptr = API.sqlite3_column_blob(handle, i);
                                int size = API.sqlite3_column_bytes(handle, i);

                                if (ptr == IntPtr.Zero)
                                {
                                    result[i].values.Add(null);
                                }
                                else
                                {
                                    result[i].values.Add(new byte[size]);
                                    Marshal.Copy(ptr, (byte[]) result[i].values[result[i].values.Count - 1], 0, size);
                                }
                                break;
                            }
                            case Column.Type.NULL:
                            {
                                result[i].values.Add(null);
                                break;
                            }
                        }
                    }

                    status = API.sqlite3_step(handle);
                } while (status == API.SQLITE_ROW);

                if(status != API.SQLITE_DONE)
                {
                    throw new StatementException(status);
                }

                for(int i = 0; i < columnCount; ++i)
                {
                    // No need to store a list full of nulls
                    if(result[i].type == Column.Type.NULL)
                    {
                        result[i].values.Clear();
                    }
                }

                return result;
            }

            ~Statement()
            {
                API.sqlite3_finalize(handle);
            }
        }
    }
}
