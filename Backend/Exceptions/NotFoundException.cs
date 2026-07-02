namespace Backend.Exceptions;

public class NotFoundException(string message) : Exception(message) {}