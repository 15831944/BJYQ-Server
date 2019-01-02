Public Class Class1

    ' compare the hashed password against the stored password
    Private Function ComparePasswords(ByVal storedPassword As Byte(), ByVal hashedPassword As Byte()) As Boolean
        If ((storedPassword Is Nothing) OrElse (hashedPassword Is Nothing) OrElse (hashedPassword.Length <> storedPassword.Length - saltLength)) Then
            Return False
        End If

        ' get the saved saltValue
        Dim saltValue(saltLength - 1) As Byte
        Dim saltOffset As Integer = storedPassword.Length - saltLength
        Dim i As Integer = 0
        For i = 0 To saltLength - 1
            saltValue(i) = storedPassword(saltOffset + i)
        Next

        Dim saltedPassword As Byte() = CreateSaltedPassword(saltValue, hashedPassword)

        ' compare the values
        Return CompareByteArray(storedPassword, saltedPassword)
    End Function

    ' compare the contents of two byte arrays
    Private Function CompareByteArray(ByVal array1 As Byte(), ByVal array2 As Byte()) As Boolean
        If (array1.Length <> array2.Length) Then
            Return False
        End If

        Dim i As Integer
        For i = 0 To array1.Length - 1
            If (array1(i) <> array2(i)) Then
                Return False
            End If
        Next

        Return True
    End Function

    ' create a salted password given the salt value
    Private Function CreateSaltedPassword(ByVal saltValue As Byte(), ByVal unsaltedPassword As Byte()) As Byte()
        ' add the salt to the hash
        Dim rawSalted(unsaltedPassword.Length + saltValue.Length - 1) As Byte
        unsaltedPassword.CopyTo(rawSalted, 0)
        saltValue.CopyTo(rawSalted, unsaltedPassword.Length)

        'Create the salted hash			
        Dim sha1 As sha1 = sha1.Create()
        Dim saltedPassword As Byte() = sha1.ComputeHash(rawSalted)

        ' add the salt value to the salted hash
        Dim dbPassword(saltedPassword.Length + saltValue.Length - 1) As Byte
        saltedPassword.CopyTo(dbPassword, 0)
        saltValue.CopyTo(dbPassword, saltedPassword.Length)

        Return dbPassword
    End Function

End Class
