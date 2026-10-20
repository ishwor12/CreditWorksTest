export const manufacturersApi = {
  getAll: async () => {
    const response = await fetch("/api/manufacturers");
    return {
      ok: response.ok,
      data: response.ok ? await response.json() : null,
    };
  },
};
